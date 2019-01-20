using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO : 엑셀과 연동하기 위하여 데이터 조정값들은 따로 관리해야 하므로 구분 하기 위해 분리
/// NOTE : 연동 부분은 아직 미구현으로 Inspector에서 값을 조정하기위해 따로 컴포넌트로 추가
/// </summary>
public class PlayerData : MonoBehaviour
{
    #region EXCEL DATA
    [Header("HEALTH OPTION"),Range(0,250)]
    public int maxHP;

    [Header("TP OPTION"), Range(0, 150)]
    public int maxTP;
    [Range(0f,1f)]
    public float recoveryTPRate;
    [Range(0f,10f)]
    public int recoveryTPAmount;
    
    /// <summary>
    /// Move :  플레이어 캐릭터 이동 함수 
    /// maxSpeedValue
    /// NOTE : 최대 속도 값 설정 (키 입력값 * maxSpeedValue (프레임당 키입력 값은 (0~1)))
    /// accelerationValue
    /// NOTE : 최대 속도 가속 값 (실제 속도 += 키입력값 * accelerationValue)
    /// decelerationValue
    /// NOTE : 정지 상태 감속 값 (Lerp함수 Time.deltaTime * decelerationValue)
    /// </summary>
    [Header("MOVE OPTION")]
    [Range(10f, 100f)]
    public float maxSpeedValue;
    [Range(5, 50f)]
    public float accelerationValue, decelerationValue;

    /// <summary>
    /// JUMP : 버튼이나 해당 키가 눌려있음을 체크하여 JumpCount만큼 Addforce(impulse) 반복 실행
    /// JumpMaxCount 
    /// NOTE : 최대 addforce를 실행할 카운트 설정
    /// JumpPowerPerCount
    /// NOTE : addforce에 실제로 들어갈 jumpPower값 설정
    /// addForceFrameIntervalSpeed 
    /// NOTE : addforce를 함수 실행 프레임을 몇초 간격으로 둘것인지 설정
    /// </summary>
    [Header("JUMP OPTION")]
    [Range(1, 10)]
    public int jumpMaxCount;
    [Range(1, 30)]
    public float jumpPowerPerCount;
    [Range(0.001f, 0.1f)]
    public float addForceFrameIntervalTime;

    /// <summary>
    /// ATTACK 관련 함수 
    /// attackEffectModel
    /// NOTE : 해당 이펙트 오브젝트 저장
    /// attackRange
    /// NOTE : 이펙트 Scale을 설정하여 처리
    /// attackDamage
    /// NOTE : 공격 데미지 양
    /// attackAnimSpeed
    /// NOTE : 애니매이션의 속도 이펙트 생성 시간 및 끝나는 시간조절
    /// attackCoolTime
    /// NOTE : 공격 후 대기시간
    /// </summary>
    [Header("ATTACK OPTION")]
    [SerializeField]
    public GameObject attackEffectModel;
    [Range(1f, 5f)]
    public float attackRange;
    [Range(1f, 10f)]
    public int attackDamage;
    [Range(0f, 5f)]
    public float attackCoolTime;
    [Range(0f,5f)]
    public float attackAnimSpeed;
    [Range(0, 100)]
    public int attackTPAmount;
    #endregion

    [Header("TAKE DAMAGE")]
    [Range(0f,10f)]
    public float invincibleTime;
    [Range(0f, 1f)]
    public float flashRate;
    [Range(0f, 500f)]
    public float knockBackPower;
}