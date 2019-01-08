using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class SetRoomInfo
//{
//    private List<DungeonRoomByTile> createdRoomList;
//    private List<List<DungeonRoomByTile>> LevelRoomList = new List<List<DungeonRoomByTile>>();
//    private Dictionary<int, List<DungeonRoomByTile>> LevelRoomDic = new Dictionary<int, List<DungeonRoomByTile>>();

//    private int levelCount;
    
//    public SetRoomInfo(List<DungeonRoomByTile> tmplist)
//    {
//        createdRoomList = tmplist; 
//        SetLevel();
//        RandomEdgeConnected();
//        PrintTest();
//    }
//    /// <summary>
//    /// 0 -> left 1, right 2?
//    /// </summary>
//    /// <param name="room1"></param>
//    /// <param name="room2"></param>
//    private void ConnectEdge(DungeonRoomByTile room1, DungeonRoomByTile room2)
//    {
//        room1.neighborRooms.Add(room2);
//        room2.neighborRooms.Add(room1);
//    }
    
//    private int setSameLevelPer = 50;
//    /// <summary>
//    /// 모든 방의 레벨 설정하고 레벨를 id로 한 2차원 리스트에 저장
//    /// </summary>
//    private void SetLevel()
//    {
//        levelCount = 0;
//        //해당 레벨의 첫번쨰 인덱스 리스트를 생성하기 위함
//        LevelRoomList.Add(new List<DungeonRoomByTile>());
        
//        for (int i = 0; i < createdRoomList.Count-1; i++)
//        {
//            createdRoomList[i].Level = levelCount;
//            LevelRoomList[createdRoomList[i].Level].Add(createdRoomList[i]);
//            //레벨를 랜덤으로 올리나 if문 적용이 안될시 확률을 높이기 위함(같은 레벨 방이 최대 3개 이하로 만들기 위해)
//            if (Random.Range(0, 100) > setSameLevelPer)
//            {
//                setSameLevelPer = 50;
//                LevelRoomList.Add(new List<DungeonRoomByTile>());
//                levelCount++;
//            }
//            else
//            {
//                setSameLevelPer -= 25;
//            }
//        }

//        LevelRoomList.Add(new List<DungeonRoomByTile>());
        
//    }
    
//    private void SetLevelInDic()
//    {
//        levelCount = 0;
//        for(int i = 0;i<createdRoomList.Count; i++)
//        {
//            createdRoomList[i].Level = levelCount;
//            if(Random.Range(0,100)>setSameLevelPer)
//            {
//                setSameLevelPer = 50;
//                levelCount++;
//            }
//            else
//            {
//                setSameLevelPer -= 25;
//            }
//        }
//    }
//    /// <summary>
//    /// 각 방들 연결
//    /// </summary>
//    private void RandomEdgeConnected()
//    {
//        //..RoomList;
//        for (int i = 0; i < LevelRoomList.Count; i++)
//        {
//            //같은 레벨 방이 2개 이상 있을 경우
//            if (LevelRoomList[i].Count >= 2)
//            {
//                for (int j = 0; j < LevelRoomList[i].Count - 1; j++)
//                    ConnectEdge(LevelRoomList[i][j], LevelRoomList[i][j + 1]);

//                if (i + 1 < LevelRoomList.Count)
//                    return;
//                //다음레벨로 길이 끊기지 않도록 한개는 우선 적용 (우선 적용할 방은 같은 레벨을 가진 방중 랜덤으로 선택)
//                var s = Random.Range(0, LevelRoomList[i].Count - 1);
//                ConnectEdge(LevelRoomList[i][s], LevelRoomList[i + 1][Random.Range(0, LevelRoomList[i + 1].Count - 1)]);

//                for (int k = 0; k < LevelRoomList[i].Count - 1; k++)
//                {
//                    s++;
//                    var tmpIndex = s % LevelRoomList[i].Count;
//                    if (Random.Range(0, 100) > 50)
//                        ConnectEdge(LevelRoomList[i][tmpIndex], LevelRoomList[i][Random.Range(0, LevelRoomList[i + 1].Count)]);
//                }
//            }
//            else if (LevelRoomList[i].Count == 1)
//            {
//                ConnectEdge(LevelRoomList[i][0], LevelRoomList[i + 1][Random.Range(0, LevelRoomList[i + 1].Count - 1)]);
//            }
//            else
//            {
//                Debug.Log(" 레벨 [" + i + "] 를 가진 방이 존재하지 않습니다.");
//            }
//        }
//    }

//    /// <summary>
//    /// 방 연결 프린트
//    /// </summary>
//    private void PrintTest()
//    {
//        for (int i = 0; i < createdRoomList.Count; i++)
//        {
//            Debug.Log("Room[" + i + "] : " + createdRoomList[i].Level);
//        }

//        for (int i = 0; i < LevelRoomList.Count; i++)
//        {
//            foreach (var t in LevelRoomList[i])
//                Debug.Log("Level [" + i + "] : " + t);
//        }

//        for (int i = 0; i < createdRoomList.Count; i++)
//        {
//            Debug.Log(i + "번째 Room Neighbor! Level : " + createdRoomList[i].Level);
//            for (int j = 0; j < createdRoomList[i].neighborRooms.Count; j++)
//            {
//                Debug.Log("[" + i + "]Room -> [" + createdRoomList[i].neighborRooms[j].roomNumber + "] Room");
//            }
//        }
//    }
//}


////레벨 0인 시작 방에서 레벨 1인 방은 모두 연결
//foreach (var r in roomList)
//{
//    if (r.weight.Equals(1))
//        ConnectEdge(roomList[0], r);
//}

//for (int i = 1; i<roomList.Count - 2; i++)
//{
//    for (int j = i + 1; j<roomList.Count - 2; j++)
//    {
//        //레벨가 같은 방을 찾아 서 연결할지 말지 랜덤으로 결정
//        if (roomList[i].weight.Equals(roomList[j].weight))
//        {
//            if (Random.Range(0, 100) > randomConnectweight)
//                ConnectEdge(roomList[i], roomList[j]);
//        }
//        //레벨가 한단계 높은방은 무조건연결?
//        else if ((roomList[i].weight + 1).Equals(roomList[j].weight))
//        {
//            ConnectEdge(roomList[i], roomList[j]);
//        }
//    }
//}