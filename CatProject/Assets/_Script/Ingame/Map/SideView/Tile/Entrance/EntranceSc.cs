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
    public EntranceSc connectedNextEntrance = null;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(unLockState)
        {
            if (collision.CompareTag("Player"))
            {
                if (collision.GetComponent<Player>().AttackOn)
                {
                    InGameManager.GetInstance().ChangeCurrentRoom(currentRoomNumber, connectedNextEntrance.currentRoomNumber);
                    collision.transform.position = connectedNextEntrance.transform.position;
                    collision.GetComponent<Player>().StopCharacter(1f);
                }
            }
        }
        
    }
}
