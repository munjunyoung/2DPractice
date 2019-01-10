using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceSc : MonoBehaviour
{
    /// <summary>
    /// NOTE : 자신이 현재 존재하는 방과 연결된 다음 방
    /// </summary>
    public bool unLockState = false;
    public int currentRoomNumber = -1;
    public int nextRoomNumber = -1;
    public Vector3 nextEntrancePos = Vector3.zero;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(unLockState)
        {
            if (collision.tag.Equals("Player"))
            {
                InGameManager.GetInstance().EnterRoom(currentRoomNumber, nextRoomNumber);
                collision.transform.position = nextEntrancePos + new Vector3Int(0,5,0);
            }
        }
        
    }
}
