using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using static Values;

// 플레이어 층 확인 필요
// 일정 이상이면 (10) 아래 위로 올리기

public partial class PlayerController : MonoBehaviour
{
    [BoxGroup("이동")]
    [SerializeField] float baseMoveSpeed;
    [BoxGroup("점프")]
    [SerializeField] int maxJumpCount;
    [BoxGroup("점프")]
    [SerializeField] float baseJumpPower;
    [BoxGroup("더블점프")]
    [SerializeField] float doubleJumpPower;         // 변하지 않는 값
    [BoxGroup("더블점프")]
    [SerializeField] float doubleJumpSpeed;         // 변하지 않는 값

    [BoxGroup("컴포넌트")]
    [SerializeField] PlayerCollider playerCollider;
    public MagnetCollider MagnetCollider => playerCollider.MagnetCollider;

    [BoxGroup("트랜스폼")]
    [SerializeField] Transform vfxParent;
    public Transform VFXParent => vfxParent;

    PlayerAnimation playerAnimation;
    CamOutside camOutside;             // Mesh가 있어야 작동

    private float baseScale;
    private float moveSpeed;
    private float jumpPower;
    private float moveResist;

    private bool isInput;            // 점프 버튼을 누름
    private bool isLanding;            // 점프 중
    private bool isEvent;          // 이벤트 중에는 캐릭터 움직이지 않게하기 위한 변수
    private Vector3 dir;

    private int jumpCount;
    private bool ignoreBomb;

    private int pointerID;                      // PC, 모바일에서 IsPointerOver가 다름

    Rigidbody2D rigid;
    Collider2D col;             // 바닥 콜라이더

    InGameManager _GameManager;

    private static readonly float _Distance_CanInputJump = 1.5f;
    private static readonly float _Distance_GroundCheck = 0.3f;
    private static readonly float _Velocity_JumpGrace = 10f;

    private static readonly float _Duration_Multiple_Start = 0.5f;


    private void Awake()
    {
        _GameManager = InGameManager.Instance;
        _AudioManager = AudioManager.Instance;

        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Instanitate Model, SFX
        CreateModel();

        moveSpeed = baseMoveSpeed;
        jumpPower = baseJumpPower;
        baseScale = transform.localScale.x;

        // OnStateChange += PlayerAction;

        invisibleObj.SetActive(false);

        playerCollider.OnAwake(this);
        playerAnimation.OnAwake(this);
    }

    void CreateModel()
    {
        var value = DataManager.Instance.Profile.Value;
        var model =  Instantiate(GameData.Instance.CharacterModel((ECharacter)value), transform);
        model.transform.localScale = Vector3.one * Scale_Ingame_Model;

        playerAnimation = model.GetComponent<PlayerAnimation>();

        // CamOutside 스크립트로 추가
        camOutside = model.transform.GetChild(1).gameObject.AddComponent<CamOutside>();
        camOutside.OnInvisible = PlayerHit;
    }

    private void Start()
    {
        dir = Random.Range(0f, 1f) < 0.5f ? Vector3.right : Vector3.left;
        transform.localScale = dir == Vector3.left ? new Vector3(-baseScale, baseScale, baseScale) : Vector3.one * baseScale;

        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ => JumpCheck())
            .AddTo(this.gameObject);

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ => JumpCheck())
            .AddTo(this.gameObject); ;

        jumpCount = 0;

#if UNITY_EDITOR
        pointerID = -1;
#else
        pointerID = 0;
#endif

        CheckUpgrade();
        CheckUseItems();
    }

    private void FixedUpdate()
    {
        if(!isEvent)
        {
            transform.Translate(dir * Time.fixedDeltaTime * moveSpeed);
        }

        if((transform.position.x <= -Limit_WallX && dir == Vector3.left)
            || (transform.position.x >= Limit_WallX && dir == Vector3.right))
        {
            ChangeDir();
        }
    }

    public void ChangeDir()
    {
        dir *= -1;
        transform.localScale = dir == Vector3.left ? new Vector3(-baseScale, baseScale, baseScale) : Vector3.one * baseScale;
    }

    private void CheckUpgrade()
    {
        moveResist = _GameManager.UpgradeValue(EUpgradeType.Resist);
    }

    private void CheckUseItems()
    {
        int startValue = 0;
        var dataManager = DataManager.Instance;

        startValue = (int)dataManager.GetUseItemTypeValue(EUseItemType.StartBoost1)
                        + (int)dataManager.GetUseItemTypeValue(EUseItemType.StartBoost2);

        ignoreBomb = (int)dataManager.GetUseItemTypeValue(EUseItemType.IgnoreBomb) > 0;

        if (startValue != 0)
        {
            MoveToSafeZone(startValue, _Duration_Multiple_Start);
        }
    }

    private void JumpCheck()
    {
        if (isEvent)
        {
            // Debug.Log("Eventing");
            return;
        }

        // Debug.Log("Input Left Button");
        //if (EventSystem.current.currentSelectedGameObject != null)
        if (EventSystem.current.IsPointerOverGameObject(pointerID))
        {
            Debug.Log("Touch UI");
            return;
        }

        if (jumpCount == maxJumpCount)
        {
            if(!isInput)
            {
                isInput = true;
                // Debug.Log("선입력 충전");
            }
            //Debug.Log($"Max : {transform.position.y}");
            Debug.Log($"Max jump count");
            return;
        }
        else if (rigid.velocity.y < -_Velocity_JumpGrace)         // 유예 시간 주기
        {
            Debug.Log($"Falling {rigid.velocity.y} - {jumpCount}");
            if(jumpCount == 0)      // 1회점프만 못하게 막기
                jumpCount++;
            //StartCoroutine(nameof(CoCheckLand));
            //return;
        }

        // Debug.Log($"Jump : {rigid.velocity.y}");

        Jump();
    }

    void Jump()
    {
        jumpCount++;

        // isInput = true;
        col.isTrigger = true;

        switch(jumpCount)
        {
            case 1:
                rigid.velocity = new Vector2(0, jumpPower);

                ChangeState(EPlayerState.Jump);
                break;
            case 2:
                rigid.velocity = new Vector2(0, doubleJumpPower);

                ChangeState(EPlayerState.DoubleJump);
                break;
        }

        if (!isLanding)
        {
            Observable.FromCoroutineValue<float>(CoCheckLand)
                .Subscribe(height => _GameManager.CheckBestHeight((int)(height / FloorPerY)))
                .AddTo(gameObject);
        }
    }

    /// <summary>
    /// 땅을 확인해서 Player jump input 활성화
    /// </summary>
    /// <returns></returns>
    IEnumerator CoCheckLand()
    {
        isLanding = true;

        while (rigid.velocity.y >= 0)
        {
            yield return null;
        }

        // Debug.LogError("정상");

        while(true)
        {
            // Debug.DrawRay(transform.position, Vector2.down * _Distance_CanInputJump, Color.red);
            var hit = Physics2D.Raycast(transform.position, Vector2.down, _Distance_CanInputJump, Layer_Floor);
            if (hit)
            {
                // 트리거를 늦게 꺼서 이미 블럭과 닿은 경우 확인
                float floorY = hit.transform.position.y;
                // Debug.Log($"Distance : {transform.position.y - floorY}");
                if (transform.position.y - floorY < _Distance_GroundCheck)
                {
                    EndJump();
                }

                Land();

                yield return floorY;
                yield break;
            }
            yield return null;
        }
    }

    public void SetSpeed(float percent = 1f)
    {
        if(percent != 1f)
        {   // 상태 이상 저항
            percent = percent > 1f ? percent - moveResist : percent + moveResist;
        }

        moveSpeed = baseMoveSpeed * percent;
    }

    private void Land()
    {
        col.isTrigger = false;
        isLanding = false;
        isInput = false;
        moveSpeed = baseMoveSpeed;
    }

    public void EndJump()
    {
        // Debug.Log($"Reset! : {jumpCount}");

        if (jumpCount == 0 || isEvent)
            return;

        // Debug.Log("CollisionToFloor");
        ChangeState(EPlayerState.Run);
    }

    /// <summary>
    /// Hit => die
    /// 그 외 Hit
    /// </summary>
    private void PlayerHit()
    {
        Debug.Log("Quit Check");
        if (GameApplication.Instance.Quitting)
        {
            return;
        }

        Debug.Log("Pause Check");
        if (InGameManager.Instance.IsPause)
        {
            return;
        }

        if (isEvent)
        {
            Debug.Log("Die");
            ChangeState(EPlayerState.Die);
        }
        else
        {   // 밖으로 나가서 죽는거면 무적 끄기
            Debug.Log("Hit");
            invisibleObj.SetActive(false);
            ChangeState(EPlayerState.Hit);
        }
    }

    public void CheckIgnoreBomb()
    {
        // 지속
        if(!ignoreBomb)
        {
            ChangeState(EPlayerState.Hit);
        }
        else
        {
            Debug.Log("방어");
        }
    }


    #region Editor

    [Button]
    [BoxGroup("이동")]
    void SetMoveSpeed()
    {
        moveSpeed = baseMoveSpeed;
    }

    [Button]
    [BoxGroup("점프")]
    void SetJumpPower()
    {
        jumpPower = baseJumpPower;
    }

#endregion
}
