using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerAnimation : MonoBehaviour
{
    [System.Serializable]
    public struct FStateVfx
    {
        public EPlayerState state;
        public GameObject obj;
    }
    public FStateVfx[] stateVfxs;
    private Dictionary<EPlayerState, GameObject> animVfxs;

    Animator anim;

    PlayerController playerController;

    private EPlayerState playerState;
    private bool isFever;

    private static readonly int _Anim_Jump = Animator.StringToHash("JumpCount");
    private static readonly int _Anim_Die = Animator.StringToHash("Die");
    private static readonly int _Anim_Fever = Animator.StringToHash("IsFever");


    public void OnAwake(PlayerController player)
    {
        playerController = player;

        anim = GetComponent<Animator>();

        animVfxs = new Dictionary<EPlayerState, GameObject>();
        foreach (var stateVfx in stateVfxs)
        {
            var vfxObj = Instantiate(stateVfx.obj, player.VFXParent);
            animVfxs.Add(stateVfx.state, vfxObj);

            vfxObj.SetActive(false);
        }

        PlayParticle(EPlayerState.Run);

        player.OnStateChange += SetAnim;
        player.OnStateChange += PlayParticle;

        player.OnFever += (value) => 
        {
            isFever = value;
            anim.SetBool(_Anim_Fever, isFever);
            if(!value)
            {   // 파티클이 없는 상태로 변경
                PlayParticle(EPlayerState.Run);
            }
        };          // false처리 때문에 연결
    }

    private void SetAnim(EPlayerState state)
    {
        if (CheckFever(state))
            state = EPlayerState.Fever;

        switch (state)
        {
            case EPlayerState.Run:
                anim.SetInteger(_Anim_Jump, 0);
                break;
            case EPlayerState.Jump:
                anim.SetInteger(_Anim_Jump, 1);
                break;
            case EPlayerState.DoubleJump:
                anim.SetInteger(_Anim_Jump, 2);
                break;
            case EPlayerState.Hit:
                anim.SetTrigger(_Anim_Die);
                break;
            case EPlayerState.Die:
                StopCoroutine(nameof(CoDieTween));
                break;
            case EPlayerState.Revive:
                transform.localPosition = Vector3.zero;
                anim.Rebind();
                anim.Update(0f);
                break;
            case EPlayerState.Fever:
                anim.SetInteger(_Anim_Jump, 0);
                anim.SetBool(_Anim_Fever, true);
                break;
        }
    }

    private void PlayParticle(EPlayerState state)
    {
        if(CheckFever(state))
        {
            state = EPlayerState.Fever;
        }

        if (animVfxs.ContainsKey(playerState))
            animVfxs[playerState].SetActive(false);

        playerState = state;

        if (animVfxs.ContainsKey(playerState))
            animVfxs[playerState].SetActive(true);
    }

    private bool CheckFever(EPlayerState state)
    {
        if (state == EPlayerState.Hit || state == EPlayerState.Die)
            return false;

        if (isFever && state != EPlayerState.Fever)
            return true;
        return false;
    }

    public void DieTween()
    {
        StartCoroutine(nameof(CoDieTween));
    }

    IEnumerator CoDieTween()
    {
        while(true)
        {
            transform.position += Vector3.down * Time.deltaTime * 8f;
            yield return null;
        }
    }
}
