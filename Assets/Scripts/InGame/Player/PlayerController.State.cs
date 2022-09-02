using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using static Values;

public partial class PlayerController
{
    public System.Action<EPlayerState> OnStateChange;
    public System.Action<bool> OnFever;
    public System.Action<bool> OnFly;
    public System.Action OnHit;
    public System.Action OnDead;
    public System.Action OnRevive;

    [BoxGroup("������")]
    [SerializeField] GameObject invisibleObj;             // �������� Obj
    [BoxGroup("������")]
    [SerializeField] GameObject springObj;
    //[BoxGroup("������")]
    //[SerializeField] GameObject feverObj;                 // Effect -> Anim

    private float feverTime;
    private float invisibleTime;
    public bool IsInvisible => invisibleTime > 0;
    public bool IsFever => feverTime > 0;

    private System.Action onInvisibleEnd;

    Sequence sequence;
    AudioManager _AudioManager;


    private void OnDestroy()
    {
        sequence.Kill();
    }

    public void ChangeState(EPlayerState state)
    {
        switch (state)
        {
            case EPlayerState.Run:
                jumpCount = 0;
                if (isInput)
                {   // *JumpCheck�� �ϴ°�� Distance JumpEnd�� ���ļ� ��� LandCheck -> Overflow �߻�
                    Debug.Log("���Է�");
                    Jump();
                }
                break;
            case EPlayerState.Hit:
                if (invisibleObj.activeSelf || isEvent)
                    return;

                _AudioManager.PlaySFX(ESfx.Death);
                OnHit?.Invoke();
                feverTime = 0f;
                SetPlayerPhysics(false);
                break;
            case EPlayerState.Die:
                // item check
                OnDead?.Invoke();
                break;
            case EPlayerState.Jump:
                _AudioManager.PlaySFX(ESfx.Jump);
                break;
            case EPlayerState.DoubleJump:
                _AudioManager.PlaySFX(ESfx.DoubleJump);
                moveSpeed = baseMoveSpeed * doubleJumpSpeed;            // �ٸ� ���ǿ� ���� �ȹ���
                break;
            case EPlayerState.Revive:
                _AudioManager.PlaySFX(ESfx.Revive);
                // ����?
                MoveToSafeZone();
                OnRevive?.Invoke();
                CameraManager.Instance.ResetVCam(() =>
                {
                    SetPlayerPhysics(true);
                    AddInvisible(Duration_Revive);
                });
                // ChangeState(EPlayerState.Run);
                break;
        }

        OnStateChange?.Invoke(state);
    }

    /// <summary>
    /// 3ĭ �������� �̵�
    /// </summary>
    public void MoveToSafeZone(int value = 0, float durationMultiple = 1f)
    {
        // Debug.Log("Moving!!");
        rigid.velocity = Vector2.zero;

        if (value > 0)
        {   // ���� �ö󰡴� ��쿡�� �ٶ����� �۵� + ���� �ִϸ��̼�
            springObj.SetActive(true);
            // jumpCount = 1;
            ChangeState(EPlayerState.Jump);
        }

        //int height = _GameManager.Height.Value;       // **�������϶��� �ƴҶ� ���� ���̰� �߻���
        int height = (int)(transform.position.y / FloorPerY);
        int destHeight = (height + value) / AllBlockPerCount * AllBlockPerCount;
        Debug.Log($"Spring : {transform.position.y} / {FloorPerY} = {height} -> {destHeight}");
        var destY = destHeight * FloorPerY;

        if (value == 0)
        {
            transform.position = new Vector2(0, destY);
            _GameManager.CheckBestHeight(destHeight);            // ���� �� ���� ���� �ö󰡸� ���������
        }
        else
        {
            OnFly?.Invoke(true);

            float duration = (destHeight - height) * Duration_Spring;
            // Debug.Log(duration);

            sequence = DOTween.Sequence();
            sequence.Append(transform.DOMoveY(destY, duration * durationMultiple)
                .OnComplete(() =>
                {
                    OnFly?.Invoke(false);
                    SetPlayerPhysics(true);
                    // height �˻�?
                    _GameManager.CheckBestHeight(destHeight);
                    StopCoroutine(nameof(CoCalcHeight));
                    springObj.SetActive(false);
                }));

            SetPlayerPhysics(false);

            StartCoroutine(nameof(CoCalcHeight));
            AddInvisible(duration * durationMultiple + durationMultiple);
        }
    }

    IEnumerator CoCalcHeight()
    {
        while (true)
        {
            yield return null;
            // Debug.Log($"�÷��̾� ���� : {transform.position.y} => {Mathf.FloorToInt(transform.position.y / Values.FloorPerY)}");
            _GameManager.CheckBestHeight(Mathf.FloorToInt(transform.position.y / FloorPerY));
        }
    }

    private void SetPlayerPhysics(bool isActive)
    {
        if (!isActive)
        {
            rigid.velocity = Vector2.zero;
        }

        rigid.simulated = isActive;
        isEvent = !isActive;
    }

    public void AddInvisible(float time)
    {
        if(invisibleTime <= 0)
        {
            invisibleTime = time;
            StartCoroutine(nameof(CoInvisibleTime));
        }
        else if (invisibleTime < time)
        {
            invisibleTime = time;
        }
    }

    IEnumerator CoInvisibleTime()
    {
        invisibleObj.SetActive(true);

        while (invisibleTime > 0)
        {
            yield return Delay01;
            invisibleTime -= 0.1f;
        }

        invisibleTime = 0;
        invisibleObj.SetActive(false);
    }

    public void Fever(float value)
    {
        if(!IsFever)
        {
            feverTime = value;
            StartCoroutine(nameof(CoFever));
        }
        feverTime = value;
    }

    IEnumerator CoFever()
    {
        ChangeState(EPlayerState.Fever);
        OnFever?.Invoke(true);

        while (feverTime > 0)
        {
            yield return Delay01;
            feverTime -= 0.1f;
            // Debug.Log(feverTime);
        }

        feverTime = 0;
        OnFever?.Invoke(false);
    }
}
