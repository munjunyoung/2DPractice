using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class InGameManager : MonoBehaviour
{
    //Singletone
    public static InGameManager instance;
    //Board
    [Header("BoardManager for CreatedRoomList")]
    public BoardManager boardmanagerSc;
    private List<DungeonRoom> roomList = new List<DungeonRoom>();
    private DungeonRoom currentRoom;
    
    //Player
    private Player playerOb;
    private string playerPath = "Prefab/Character/Player/";
    //Stop
    private float changeRoomStopCount = 2f;

    private void Awake()
    {
        //Singletone
        instance = this;
        //캐릭터 생성
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
        boardmanagerSc.CreateRooms();
        roomList = boardmanagerSc.roomList;
        roomList[0].roomModel.SetActive(true);
        currentRoom = roomList[0];
        currentRoom.CheckLockRoom();
        //시작할땐 0번 방이므로 체크
        
        CreatePlayer(GlobalManager.instance.pType, playerPath);
    }
    
    /// <summary>
    /// 플레이어 생성
    /// </summary>
    private void CreatePlayer(PLAYER_TYPE _playertype, string path)
    {
        var playerprefab = Resources.Load(path + _playertype.ToString()) as GameObject;
        playerOb = Instantiate(playerprefab).GetComponent<Player>();
    }

    #region ROOM
    /// <summary>
    /// NOTE : 출입문에 진입시 해당 함수를 호출 currentnum와 nextnum는 출입문 오브젝트를 생성할때 저장해둔 변수값
    /// TODO : 해당 함수를 EntranceSc 트리거함수에 구현 하는게되면 roomList를 여러군데에서 참조하게 될것같아서 변경
    /// </summary>
    /// <param name="_currentroom"></param>
    /// <param name="_nextroom"></param>
    public void ChangeCurrentRoom(DungeonRoom _currentroom, DungeonRoom _nextroom)
    {
        _currentroom.roomModel.SetActive(false);
        _nextroom.roomModel.SetActive(true);
        currentRoom = _nextroom;
        Debug.Log("Change Room ! : CurrentRoom[" + _currentroom.roomNumberOfList + "]" +" Type : " + _currentroom.roomClearType + " -> [" + _nextroom.roomNumberOfList + "]" + "Type : " + _nextroom.roomClearType);
        currentRoom.CheckLockRoom();

        StopAllCharacter(changeRoomStopCount);
    }
    
    #endregion
    
    /// <summary>
    /// NOTE : 파리미터가 true이면 TimeScale값을 0으로 변경 , 반대면 1
    /// </summary>
    /// <param name="_ispause"></param>
    private void TimePause(bool _ispause)
    {
        Time.timeScale = _ispause ? 0f : 1f;
    }
    
    /// <summary>
    /// NOTE : 방이 변경되었을때 캐릭터와 몬스터 파라미터 값만큼 정지
    /// </summary>
    private void StopAllCharacter(float _stopcount)
    {
        playerOb.StopAction(_stopcount);
        currentRoom.MonsterStop(_stopcount);
        currentRoom.ItemStop(_stopcount);
        
        
    }

    /// <summary>
    /// NOTE : Stage Clear, 알람패널 실행
    /// </summary>
    public void CheckStageClear()
    {
        PlayerUIManager.instance.ShowAlaramPanel(Alarm_State.CLEAR);
    }

    /// <summary>
    /// NOTE : 플레이어 죽음 (클리어 실패) 알람 패널 실행
    /// </summary>
    public void DiePlayer()
    {
        PlayerUIManager.instance.ShowAlaramPanel(Alarm_State.DIE);
    }
}