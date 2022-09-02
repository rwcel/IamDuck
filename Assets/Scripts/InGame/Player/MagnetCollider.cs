using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 플레이어의 마그넷 콜라이더 켜주기
/// 일정 시간동안 먹는 물체들 플레이어 쪽으로 이동 시켜서 적용시키기 => Trigger 알아서 적용
/// </summary>
public class MagnetCollider : MonoBehaviour
{
    private float offtime;
    private bool isActive;

    private static readonly float delay = 1f;

    private System.Action OnReset;

    public void Active(float time)
    {
        offtime = time;

        // Debug.Log($"지속시간 : {offtime}");

        if(!isActive)
        {
            isActive = true;
            gameObject.SetActive(true);

            StartCoroutine(nameof(CoMagnet));
        }
    }

    /// <summary>
    /// *아이템을 여러개 먹었을 경우에 지속시간을 초기화 해야하기 떄문에 코루틴 사용
    /// </summary>
    IEnumerator CoMagnet()
    {
        while(offtime > 0)
        {
            offtime -= delay;
            yield return Values.Delay1;
        }

        EndMagnet();
    }

    void EndMagnet()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out ItemController item))
        {
            item.Dragged(transform);

            OnReset += item.ResetDrag;
        }
    }

    public void ResetDraggingItems()
    {
        OnReset?.Invoke();
        OnReset = null;

        EndMagnet();
    }
}
