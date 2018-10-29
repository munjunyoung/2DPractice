using UnityEngine;

public class Player : MonoBehaviour
{
    enum MoveState { Idle, Walk, Run }
    enum Key { W = 0, S, A, D }

    //키코드 설정
    private bool[] dirKey = new bool[4];

    //JoyStick
    public JoyStickScript joyStickSc;

    //이동 관련
    [Range(0, 5f)]
    public float speed;
    private Vector3 directionVector = Vector3.zero;
    
    //Animation 관련
    private MoveState PlayerMoveState;
    private Animator anim;
    private byte dirBlendValue;

    // Use this for initialization
    void Start()
    {
        PlayerMoveState = MoveState.Idle;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        MoveInput();
        InputJoyStick();
    }

    private void FixedUpdate()
    {
        Move(directionVector);
        SetMoveAnimator();
    }

    /// <summary>
    /// 이동 관련 함수
    /// </summary>
    private void Move(Vector3 dir)
    {
        //대각선 일정수치 설정
        dir = dir.normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, -dir);
        transform.position += dir * speed * Time.deltaTime;
    }
    
    /// <summary>
    /// 애니매이션 셋팅 
    /// dirBlendValue - 방향 param 설정
    /// playerMoveState - 상태 param 설정 
    /// </summary>
    private void SetMoveAnimator()
    {
        //anim.SetFloat("Dir", dirBlendValue);
        // Animation Setting State Float param  0 : idle 1 : walk
        PlayerMoveState = directionVector.Equals(Vector2.zero) ? MoveState.Idle : MoveState.Walk;
        anim.SetInteger("State", (int)PlayerMoveState);
    }

    /// <summary>
    /// 키보드 키 입력 관련 함수
    /// </summary>
    private void MoveInput()
    {
        //이렇게 되면 성능이 좋지않을듯 매프레임마다 초기화 됨
        //DirKey[(int)Key.W] = Input.GetKey(KeyCode.W) ? true : false;

        if (Input.GetKeyDown(KeyCode.W))
        {
            dirKey[(int)Key.W] = true;
            dirBlendValue = (byte)Key.W;
        }
        else if (Input.GetKeyUp(KeyCode.W))
            dirKey[(int)Key.W] = false;

        if (Input.GetKeyDown(KeyCode.S))
        {
            dirKey[(int)Key.S] = true;
            dirBlendValue = (byte)Key.S;
        }
        else if (Input.GetKeyUp(KeyCode.S))
            dirKey[(int)Key.S] = false;

        if (Input.GetKeyDown(KeyCode.A))
        {
            dirKey[(int)Key.A] = true;
            dirBlendValue = (byte)Key.A;
        }
        else if (Input.GetKeyUp(KeyCode.A))
            dirKey[(int)Key.A] = false;

        if (Input.GetKeyDown(KeyCode.D))
        {
            dirKey[(int)Key.D] = true;
            dirBlendValue = (byte)Key.D;
        }
        else if (Input.GetKeyUp(KeyCode.D))
            dirKey[(int)Key.D] = false;

        var w = dirKey[(int)Key.W] ? 1 : 0;
        var s = dirKey[(int)Key.S] ? -1 : 0;
        var a = dirKey[(int)Key.A] ? -1 : 0;
        var d = dirKey[(int)Key.D] ? 1 : 0;
        
        directionVector = new Vector2(a + d, w + s);
    }

    /// <summary>
    /// 조이스틱 입력
    /// </summary>
    private void InputJoyStick()
    {
        if (!joyStickSc.GetStickDirection().Equals(Vector2.zero))
            directionVector = joyStickSc.GetStickDirection();
    }
}



