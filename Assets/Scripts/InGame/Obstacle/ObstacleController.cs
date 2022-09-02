using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Values;

public class ObstacleController : MonoBehaviour
{
    private EObstacleType type;
    public EObstacleType Type => type;

    protected InGameManager _GameManager;
    protected PoolingManager _PoolingManager;

    protected ObstacleData data;
    protected Sequence sequence;

    public System.Action OnActive;

    private bool isHit;


    private void Start()
    {
        _GameManager = InGameManager.Instance;
        _PoolingManager = PoolingManager.Instance;
    }

    public void InitialiseWithData(ObstacleData data)
    {
        this.data = data;
        type = data.type;

        // spawnPoint

        Init();
    }

    /// <summary>
    /// 局聪皋捞记 犁积 殿 贸府
    /// </summary>
    protected virtual void Init()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        isHit = false;
        // PlaySequence
    }

    protected virtual void PlaySequence() 
    {
        sequence = DOTween.Sequence();
    }

    protected void OnDisable()
    {
        sequence.Kill();

        Clear();
    }
    
    protected virtual void Clear() { }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerController player))
        {
            if(player.IsFever)
            {
                if (isHit)
                    return;

                sequence.Kill();

                _PoolingManager.Dequeue(Key_Effect_Coin, transform.position, Quaternion.identity);
                _GameManager.AddCoin(Coin_PowerPotion);

                HitObstacle(player.transform);

                isHit = true;
            }
            else
            {
                player.ChangeState(EPlayerState.Hit);
                // Debug.Log($"hitObstacle : {gameObject.name}");
            }
        }
    }

    protected virtual void HitObstacle(Transform target)
    {
        bool toRight = target.position.x < transform.position.x;

        float time = toRight ? Limit_OutsideWallX - transform.position.x : transform.position.x + Limit_WallX;
        time = Mathf.Clamp(time * 0.2f, 0.3f, 0.7f);
        // Debug.Log(time);

        sequence = DOTween.Sequence();

        sequence.Append(transform.DOMoveX(toRight ? Limit_OutsideWallX : -Limit_OutsideWallX, time)
            .SetEase(Ease.OutQuad));

        sequence.Join(transform.DOMoveY(transform.position.y + 5f, time)
            .SetEase(Ease.OutQuad));

        sequence.Join(transform.DORotate(new Vector3(0f, 0f, 360f), 0.5f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart));

        sequence.Join(transform.DOScale(0, time))
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                _PoolingManager.Dequeue(Key_Effect_Explosion, transform.position, Quaternion.identity);
                _PoolingManager.Enqueue(gameObject, true);
                transform.localScale = Vector3.one;
                OnActive?.Invoke();
            });
    }
}