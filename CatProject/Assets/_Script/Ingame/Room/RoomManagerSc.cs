using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManagerSc : MonoBehaviour
{

    private List<DungeonRoomByTile> roomList;

    private int weightCount = 0;
    
    private void Start()
    {
        roomList = GameObject.Find("BoardManager").GetComponent<BoardManagerByTile>().roomList;
        SetWeight();
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

    private void SetWeight()
    {
        //가중치가 0인방이 여러개가 되지 않도록
        roomList[0].weight = weightCount;
        weightCount++;

        for (int i = 1; i < roomList.Count-2; i++)
        {
            roomList[i].weight = weightCount;
            //가중치를 랜덤으로 올림
            if (Random.Range(0, 100) > 50)
            {
                //보스방 이전 마지막 방일경우 카운트가 올라가지 않도록
                if(i.Equals(roomList.Count-2))
                weightCount++;
            }
        }

        roomList[roomList.Count-1].weight = weightCount++;
    }

    
    private int randomConnectweight = 50;
    private void RandomEdgeConned()
    {
        //가중치 0인 시작 방에서 가중치 1인 방은 모두 연결
        foreach (var r in roomList)
        {
            if (r.weight.Equals(1))
                ConnectEdge(roomList[0], r);
        }
        
        for(int i=1; i<roomList.Count-2;i++)
        {
            for(int j=i+1; j<roomList.Count-2;j++)
            {
                //가중치가 같은 방을 찾아 서 연결할지 말지 랜덤으로 결정
                if (roomList[i].weight.Equals(roomList[j].weight))
                {
                    if (Random.Range(0, 100) > randomConnectweight)
                        ConnectEdge(roomList[i], roomList[j]);
                }
                //가중치가 한단계 높은방은 무조건연결?
                else if((roomList[i].weight+1).Equals(roomList[j].weight))
                {
                    ConnectEdge(roomList[i], roomList[j]);
                }
            }
        }
    }
}

