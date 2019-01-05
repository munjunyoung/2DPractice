using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManagerSc : MonoBehaviour
{
    private List<DungeonRoomByTile> roomList;

    private List<List<DungeonRoomByTile>> WeightRoomList = new List<List<DungeonRoomByTile>>();
    private int weightCount;


    private void Start()
    {
        roomList = GameObject.Find("BoardManager").GetComponent<BoardManagerByTile>().roomList;
        SetWeight();
        RandomEdgeConnected();
        PrintTest();
    }
    /// <summary>
    /// 0 -> left 1, right 2?
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    private void ConnectEdge(DungeonRoomByTile room1, DungeonRoomByTile room2)
    {
        room1.neighborRooms.Add(room2);
        room2.neighborRooms.Add(room1);
    }


    [Header("Set Same Weight Per"), SerializeField, Range(0, 100)]
    private int randomWeight = 50;
    /// <summary>
    /// 모든 방의 가중치 설정하고 가중치를 id로 한 2차원 리스트에 저장
    /// </summary>
    private void SetWeight()
    {
        weightCount = 0;
        WeightRoomList.Add(new List<DungeonRoomByTile>());

        for (int i = 0; i < roomList.Count; i++)
        {
            roomList[i].weight = weightCount;
            WeightRoomList[roomList[i].weight].Add(roomList[i]);
            //가중치를 랜덤으로 올리나 if문 적용이 안될시 확률을 높이기 위함(같은 가중치 방이 최대 3개 이하로 만들기 위해)
            if (Random.Range(0, 100) > randomWeight)
            {
                randomWeight = 50;
                WeightRoomList.Add(new List<DungeonRoomByTile>());
                weightCount++;
            }
            else
            {
                randomWeight -= 25;
            }
        }
    }

    //지역 변수로 선언하려하다가 방의 랜덤성에 연결 퍼센티지를 조정하고싶어서 클래스 변수로 선언
    [Header("Set Connect Percentage")]
    [SerializeField, Range(0, 100)]
    private int randomConnectSameWeightPer = 50;
    /// <summary>
    /// 각 방들 연결
    /// </summary>
    private void RandomEdgeConnected()
    {
        //..weightRoomList;
        for (int i = 0; i < WeightRoomList.Count; i++)
        {
            //같은 가중치 방이 2개 이상 있을 경우
            if (WeightRoomList[i].Count >= 2)
            {
                //1,2,3 이 있을경우 1,2 1,3 2,3 
                for (int j = 0; j < WeightRoomList[i].Count - 1; j++)
                {
                    for (int k = j + 1; k < WeightRoomList[i].Count; k++)
                    {
                        if (WeightRoomList[i][k].neighborRooms.Count == 0)
                        {
                            ConnectEdge(WeightRoomList[i][j], WeightRoomList[i][k]);
                        }
                        else
                        {
                            if (Random.Range(0, 100) <= randomConnectSameWeightPer)
                                ConnectEdge(WeightRoomList[i][j], WeightRoomList[i][k]);
                        }

                        if (i + 1 < weightCount)
                            ConnectEdge(WeightRoomList[i][0], WeightRoomList[i + 1][Random.Range(0, WeightRoomList[i + 1].Count)]);
                    }
                }
            }
            else if (WeightRoomList[i].Count == 1)
            {
                if (i + 1 < weightCount)
                    ConnectEdge(WeightRoomList[i][0], WeightRoomList[i + 1][Random.Range(0, WeightRoomList[i + 1].Count)]);
            }
            else
            {
                Debug.Log(" 가중치 [" + i + "] 를 가진 방이 존재하지 않습니다.");
            }
        }
    }

    /// <summary>
    /// 방 연결 프린트
    /// </summary>
    private void PrintTest()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Debug.Log("Room[" + i + "] : " + roomList[i].weight);
        }

        for (int i = 0; i < WeightRoomList.Count; i++)
        {
            foreach (var t in WeightRoomList[i])
                Debug.Log("Weight [" + i + "] : " + t);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            Debug.Log(i + "번째 Room Neighbor! Weight : " + roomList[i].weight);
            for (int j = 0; j < roomList[i].neighborRooms.Count; j++)
            {
                Debug.Log("[" + i + "]Room -> [" + roomList[i].neighborRooms[j].roomNumber + "] Room");
            }

        }
    }
}


////가중치 0인 시작 방에서 가중치 1인 방은 모두 연결
//foreach (var r in roomList)
//{
//    if (r.weight.Equals(1))
//        ConnectEdge(roomList[0], r);
//}

//for (int i = 1; i<roomList.Count - 2; i++)
//{
//    for (int j = i + 1; j<roomList.Count - 2; j++)
//    {
//        //가중치가 같은 방을 찾아 서 연결할지 말지 랜덤으로 결정
//        if (roomList[i].weight.Equals(roomList[j].weight))
//        {
//            if (Random.Range(0, 100) > randomConnectweight)
//                ConnectEdge(roomList[i], roomList[j]);
//        }
//        //가중치가 한단계 높은방은 무조건연결?
//        else if ((roomList[i].weight + 1).Equals(roomList[j].weight))
//        {
//            ConnectEdge(roomList[i], roomList[j]);
//        }
//    }
//}