using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceSc : StructureObject
{
    [HideInInspector]
    public EntranceSc connectedNextEntrance = null;
    public bool isBossRoomEntrance = false;

    protected override void Awake()
    {
        base.Awake();
        spriteArray = new Sprite[2];
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isBossRoomEntrance)
        {
            if (collision.CompareTag("Player"))
            {
                if (collision.GetComponent<Player>().attackButtonPress)
                {
                    if (collision.GetComponent<Player>().IsGetKey)
                    {
                        if (!StateOn)
                        {
                            if (spriteArray[1] != null)
                                sR.sprite = spriteArray[1];
                            StateOn = true;
                            return;
                        }
                    }
                }
            }
        }

        if (StateOn)
        {
            if (collision.CompareTag("Player"))
            {
                if (collision.GetComponent<Player>().attackButtonPress)
                {
                    if (connectedNextEntrance == null)
                    {
                        Debug.Log("연결된 방이 없습니다.");
                        return;
                    }

                    InGameManager.GetInstance().ChangeCurrentRoom(ownRoom, connectedNextEntrance.ownRoom);
                    collision.transform.position = connectedNextEntrance.transform.position;
                }
            }
        }

    }

    /// <summary>
    /// NOTE : UNLOCK ENTRANCE
    /// NOTE : 스프라이트 변경  sprtiearray 0 번이 닫힌문 1번이 열린문
    /// </summary>
    public void UnLockEntrance()
    {
        if (isBossRoomEntrance)
            return;
        if (spriteArray[1] != null)
            sR.sprite = spriteArray[1];
        else
            Debug.Log("Entrance Open Sprite가 존재하지 않습니다.");
        //sprite 변경 및 
        StateOn = true;
    }

    /// <summary>
    /// NOTE : 문 잠 
    /// </summary>
    public void LockEntracne()
    {
        sR.sprite = spriteArray[0];
        StateOn = false;
    }

    /// <summary>
    /// NOTE : 열쇠조건을 걸기위한 설정
    /// </summary>
    public void SetKeyEntanceToBossRoom()
    {
        isBossRoomEntrance = true;
        sR.color = Color.red;
    }

    /// <summary>
    /// NOTE : 이미 들어가있으므로 열기위한 조건은 보스클리어뿐
    /// </summary>
    public void SetBossRoomEntrance()
    {
        sR.color = Color.red;
    }
}
