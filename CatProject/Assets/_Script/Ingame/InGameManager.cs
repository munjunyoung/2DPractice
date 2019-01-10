using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoardManagerByTile))]
public class InGameManager : MonoBehaviour
{
    private static InGameManager instance;

    [Header("BoardManager for CreatedRoomList"), SerializeField]
    private BoardManagerByTile boardmanagerSc;

    private List<DungeonRoomByTile> roomList = new List<DungeonRoomByTile>();
    private DungeonRoomByTile currentRoom;
    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LoadRoomList();
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
    /// NOTE : BoardManager에서 RoomList들을 가져옴
    /// TODO : 씬을 넘길때 재활용성이 있으므로 함수로 처리
    /// </summary>
    private void LoadRoomList()
    {
        roomList = boardmanagerSc.roomList;
        roomList[0].roomModel.SetActive(true);
        currentRoom = roomList[0];
    }
    
    /// <summary>
    /// NOTE : 출입문에 진입시 해당 함수를 호출 currentnum와 nextnum는 출입문 오브젝트를 생성할때 저장해둔 변수값
    /// </summary>
    /// <param name="currentnum"></param>
    /// <param name="nextnum"></param>
    public void EnterRoom(int currentnum, int nextnum)
    {
        if(currentRoom.roomNumberOfList.Equals(currentnum))
        {
            roomList[nextnum].roomModel.SetActive(true);

            roomList[currentnum].roomModel.SetActive(false);
            currentRoom = roomList[nextnum];
        }
        else
        {
            Debug.Log("현재 방과 출입문에 저장된 현재방의 NUMBER가 다릅니다.");
        }
    }

    /// <summary>
    /// NOTE : 적을 모두 죽이거나 방에 저장되어있는 해당 요건을 만족했을경우 해제
    /// NOTE : 
    /// </summary>
    /// <param name="num"></param>
    public void UnLockRoom()
    {
        roomList[currentRoom.roomNumberOfList].unLockState = true;
        foreach(var tmpentrance in roomList[currentRoom.roomNumberOfList].neighborRooms)
            tmpentrance.entrance.GetComponent<EntranceSc>().unLockState = true;

        Debug.Log("UNLOCK ROOM [" + currentRoom.roomNumberOfList + "]");
    }
}
