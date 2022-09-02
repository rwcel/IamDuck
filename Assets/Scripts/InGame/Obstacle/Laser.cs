using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Obstacle
{
    public class Laser : ObstacleController
    {
        [SerializeField] Animator anim;
        [SerializeField] AnimEvent animEvent; 
        [SerializeField] GameObject activeObject;

        WaitForSeconds delay;

        protected override void Init()
        {
            base.Init();

            delay = new WaitForSeconds(data.value);

            activeObject.SetActive(false);
            anim.speed = 0f;
            animEvent.SetAnimEvent(() => StartCoroutine(nameof(CoReadyToActive)));

            StartCoroutine(nameof(CoActive));
        }

        IEnumerator CoActive()
        {
            yield return new WaitForSeconds(Random.Range(0f, 1.5f));

            anim.speed = 1f;

            StartCoroutine(nameof(CoReadyToActive));
        }

        IEnumerator CoReadyToActive()
        {
            activeObject.SetActive(false);

            yield return delay;

            activeObject.SetActive(true);

            // sequence.Append(DOVirtual.DelayedCall(data.value, () => activeObject.SetActive(true)))       // *비었다고 적용 안됨

        }
    }
}
