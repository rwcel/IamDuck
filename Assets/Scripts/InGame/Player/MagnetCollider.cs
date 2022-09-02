using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// �÷��̾��� ���׳� �ݶ��̴� ���ֱ�
/// ���� �ð����� �Դ� ��ü�� �÷��̾� ������ �̵� ���Ѽ� �����Ű�� => Trigger �˾Ƽ� ����
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

        // Debug.Log($"���ӽð� : {offtime}");

        if(!isActive)
        {
            isActive = true;
            gameObject.SetActive(true);

            StartCoroutine(nameof(CoMagnet));
        }
    }

    /// <summary>
    /// *�������� ������ �Ծ��� ��쿡 ���ӽð��� �ʱ�ȭ �ؾ��ϱ� ������ �ڷ�ƾ ���
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
