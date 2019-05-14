using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceSc : MonoBehaviour
{
    /// <summary>
    /// NOTE : 자신이 현재 존재하는 방과 연결된 다음 방
    /// </summary>
    private bool unLockState = false;
    [HideInInspector]
    public int currentRoomNumber = -1;
    [HideInInspector]
    public EntranceSc connectedNextEntrance = null;
    [HideInInspector]
    public Sprite doorOpenSprite = null;
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(unLockState)
        {
            if (collision.CompareTag("Player"))
            {
                if (collision.GetComponent<Player>().attackButtonPress)
                {
                    InGameManager.GetInstance().ChangeCurrentRoom(currentRoomNumber, connectedNextEntrance.currentRoomNumber);
                    collision.transform.position = connectedNextEntrance.transform.position;
                }
            }
        }
    }

    /// <summary>
    /// NOTE : UNLOCK ENTRANCE
    /// NOTE : 스프라이트 변경
    /// </summary>
    public void UnLockEntrance()
    {
        if (doorOpenSprite != null)
            GetComponent<SpriteRenderer>().sprite = doorOpenSprite;
        else
            Debug.Log("Entrance Open Sprite가 존재하지 않습니다.");
        //sprite 변경 및 
        unLockState = true;
    }
}
