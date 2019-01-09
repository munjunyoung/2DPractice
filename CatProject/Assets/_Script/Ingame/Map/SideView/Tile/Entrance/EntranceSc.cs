using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceSc : MonoBehaviour
{
    public DungeonRoomByTile currentRoom = null;
    public DungeonRoomByTile nextRoom = null;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (currentRoom.unLockState)
        {
            if (collision.tag.Equals("Player"))
            {
                    currentRoom.objectModel.SetActive(false);
                    nextRoom.objectModel.SetActive(true);
                    BoardManagerByTile.GetInstance().currentRoomNumber = nextRoom.roomNumberOfList;
                    Vector3 entrancepos = Vector3.zero;
                    foreach (var tmp in nextRoom.neighborRooms)
                    {
                        if (tmp.connectedRoom == currentRoom)
                            entrancepos = tmp.entranceOb.transform.position;
                    }
                    collision.transform.position = entrancepos + new Vector3(0f,5f,0f);
            }
        }
        
    }
}
