using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Obstacle
{
    public class Missile : ObstacleController
    {
        bool isClear;

        protected override void Init()
        {
            base.Init();

            isClear = false;

            transform.localScale = (transform.position.x < 0) ? Vector3.one : new Vector3(-1, 1, 1);

            Invoke(nameof(PlaySequence), Random.Range(0f, 1.5f));
        }

        protected override void PlaySequence()
        {
            if(isClear)
                return;

            base.PlaySequence();

            // Debug.Log(transform.position.x);

            sequence.Append(transform.DOMoveX(-transform.position.x, data.value)
                .SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart);
        }

        protected override void Clear()
        {
            base.Clear();

            isClear = true;
        }
    }
}