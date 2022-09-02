using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemController : MonoBehaviour
{
    private EIngameItem type;
    public EIngameItem Type => type;

    protected InGameManager _GameManager;
    protected PoolingManager _PoolingManager;

    protected InGameItemData itemData;
    protected float itemValue;                // Upgrade까지 적용한 value

    protected Sequence sequence;

    private Transform target;

    public System.Action OnActive;

    private void Start()
    {
        _GameManager = InGameManager.Instance;
        _PoolingManager = PoolingManager.Instance;

    }

    public void InitialiseWithData(InGameItemData itemData)
    {
        if (this.itemData == null)
        {
            this.itemData = itemData;
            type = itemData.type;
            itemValue = itemData.value;

            CheckUpgrade();
        }

        // 애니메이션 재생 등 처리
        Init();
    }

    protected virtual void CheckUpgrade() { }

    protected abstract void Init();

    protected virtual void PlaySequence()
    {
        //sequence = DOTween.Sequence();

        //transform.DOScale(1.08f, 0.5f)
        //    .SetEase(Ease.InSine)
        //    .SetLoops(-1, LoopType.Yoyo);
    }

    protected void OnDisable()
    {
        sequence.Kill();

        // Clear();
    }

    /// <summary>
    /// 플레이어와 닿은 경우
    /// </summary>
    public virtual void Active(PlayerController player)
    {
        _PoolingManager.Dequeue(Values.Key_Effect_GetItem, transform.position, Quaternion.identity);

        _PoolingManager.Enqueue(gameObject, true);

        OnActive?.Invoke();
        // OnActive = null;

        if (target != null)
            target = null;

        _GameManager.AddItem(type);
        //DataManager.Instance.OnMission_AddInGameItem?.Invoke(type, 1);
    }

    /// <summary>
    /// Target = Player
    /// </summary>
    public void Dragged(Transform target)
    {
        this.target = target;

        if (gameObject.activeSelf)
            StartCoroutine(nameof(CoDrag));
    }

    IEnumerator CoDrag()
    {
        if (target == null)
            yield break;

        while(true)
        {
            //transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * 4f);
            transform.position = Vector2.MoveTowards(transform.position, target.position, Time.deltaTime * 10f);
            yield return null;
        }
    }

    public void ResetDrag()
    {
        if (target == null)
            return;

        StopCoroutine(nameof(CoDrag));
        transform.localPosition = Vector2.zero;
    }

}
