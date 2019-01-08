using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceSc : MonoBehaviour
{
    public DungeonRoomByTile currentRoom = null;
    public DungeonRoomByTile nextRoom = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentRoom.unLockState)
        {
            if (collision.tag.Equals("Player"))
            {
                currentRoom.roomOwnObject.SetActive(false);
                nextRoom.roomOwnObject.SetActive(true);
                Vector3 entrancepos = Vector3.zero;
                foreach (var tmp in nextRoom.neighborRooms)
                {
                    if (tmp.connectedRoom == currentRoom)
                        entrancepos = tmp.entranceOb.transform.position;
                }
                collision.transform.position = entrancepos + new Vector3(0f,10,0);
            }
        }
        
    }
}
