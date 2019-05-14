using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngameCam : MonoBehaviour
{
    [Header("Cam Target")]
    private Transform playerTarget;
    [SerializeField]
    private Camera cam;

    [Header("CAM POS OPTION")]
    [SerializeField, Range(0, 10)]
    private float camUpOffset;
    [SerializeField, Range(0,20)]
    private float camDistanceOffset;

    private void Start()
    {
        playerTarget = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    { 
        if(playerTarget!=null)
            TrackingPlayer();
    }
    /// <summary>
    /// NOTE : 카메라 플레이어 추적 함수
    /// </summary>
    private void TrackingPlayer()
    {
        cam.orthographicSize = camDistanceOffset;
        Vector3 targetCampos = playerTarget.position+ Vector3.back + (Vector3.up * camUpOffset);
        transform.position = targetCampos;
    }
}


//var pos = Vector3.Lerp(transform.position, targetCampos, Time.deltaTime * camSmoothing);
//0.5f들은 타일 피벗이 센터이므로 나중에 수정해야 할 것 같다.
//pos.x = Mathf.Clamp(pos.x, leftBound-0.5f, rightBound-0.5f);
//pos.y = Mathf.Clamp(pos.y, bottomBound-0.5f, topBound-0.5f);
////카메라 맵 고정관련
//private float verticalExtent;
//private float horizontalExtent;

//private float leftBound;
//private float rightBound;
//private float bottomBound;
//private float topBound;

//[HideInInspector]
//public Rect currentRoomRect;
///// <summary>
///// 카메라가 맵 밖으로 나가지 않게하기위해 bound값을 설정
///// </summary>
//public void SetBoundValue()
//{

//    verticalExtent = Camera.main.orthographicSize;
//    horizontalExtent = verticalExtent * Screen.width / Screen.height;

//    leftBound = (float)(horizontalExtent);
//    rightBound = (float)((currentRoomRect.xMax) - horizontalExtent);
//    bottomBound = verticalExtent;
//    topBound = (currentRoomRect.yMax) - verticalExtent;
//}
