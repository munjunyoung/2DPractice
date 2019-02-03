using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardManager))]
public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    [Header("BoardManager for CreatedRoomList")]
    public BoardManager boardmanagerSc;

    private List<DungeonRoom> roomList = new List<DungeonRoom>();
    public DungeonRoom currentRoom;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartSettingInGM();
    }
    
    /// <summary>
    /// Signletone
    /// </summary>
    /// <returns></returns>
    public static InGameManager GetInstance()
    {
        return instance;
    }

    /// <summary>
    /// NOTE : BoardManager에서 RoomList들을 가져옴 시작 방인 0번방을 active On
    /// NOTE : 방 LIST들을 순회하여 각 방의 몬스터들의 숫자를 체크하여 가장 많은 몬스터 숫자만큼 HP SLIDER 생성
    /// TODO : 씬을 넘길때 재활용성이 있으므로 함수로 처리
    /// </summary>
    private void StartSettingInGM()
    {
        roomList = boardmanagerSc.roomList;
        roomList[0].roomModel.SetActive(true);
        currentRoom = roomList[0];
    }

    /// <summary>
    /// NOTE : 출입문에 진입시 해당 함수를 호출 currentnum와 nextnum는 출입문 오브젝트를 생성할때 저장해둔 변수값
    /// TODO : 해당 함수를 EntranceSc 트리거함수에 구현 하는게되면 roomList를 여러군데에서 참조하게 될것같아서 변경
    /// </summary>
    /// <param name="currentnum"></param>
    /// <param name="nextnum"></param>
    public void ChangeCurrentRoom(int currentnum, int nextnum)
    {
        roomList[currentnum].roomModel.SetActive(false);
        roomList[nextnum].roomModel.SetActive(true);
        currentRoom = roomList[nextnum];
        Debug.Log("Change Room ! : CurrentRoom[" + currentnum + "] -> [" + nextnum + "]");
    }

    /// <summary>
    /// NOTE : 적을 모두 죽이거나 방에 저장되어있는 해당 요건을 만족했을경우 해제
    /// </summary>
    /// <param name="num"></param>
    private void UnLockRoom()
    {
        roomList[currentRoom.roomNumberOfList].unLockState = true;
        foreach (EntranceConnectRoom tmpentrance in roomList[currentRoom.roomNumberOfList].neighborRooms)
            tmpentrance.entrance.GetComponent<EntranceSc>().UnLockEntrance();

        Debug.Log("UNLOCK ROOM [" + currentRoom.roomNumberOfList + "]");
    }

    /// <summary>
    /// NOTE : ROOM CHECK UN LOCK
    /// TODO : 현재는 몬스터의 존재 유무로만 LOCK 해제, 이후에 추가적으로 방의 타입에 따라 룸을 해제하는 방식을 변경 해야한다.
    /// </summary>
    public void CheckUnLockRoom()
    {
        bool checkmonsterisAlive = false;
        foreach(var monster in currentRoom.monsterList)
        {
            if (monster.isAlive)
                checkmonsterisAlive = true;
        }

        //모두 죽었을 경우 방 해제
        if (!checkmonsterisAlive)
            UnLockRoom();
    }
}