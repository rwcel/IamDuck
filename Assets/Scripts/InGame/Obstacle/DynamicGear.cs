using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Obstacle
{
    public class DynamicGear : ObstacleController
    {
        bool isClear;

        protected override void Init()
        {
            base.Init();

            isClear = false;

            Invoke(nameof(PlaySequence), Random.Range(0f, 1.5f));
        }

        protected override void PlaySequence()
        {
            if (isClear)
                return;

            base.PlaySequence();

            sequence.Append(transform.DOMoveX(-transform.position.x, data.value)
                .SetEase(Ease.Linear));

            /*sequence.Join(transform.DORotate(new Vector3(0, 0, 360), data.value, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear));
            */

            sequence.SetLoops(-1, LoopType.Yoyo);
        }

        protected override void Clear()
        {
            base.Clear();

            isClear = true;
        }

        //public override void ActiveItem(PlayerController player)
        //{
        //    base.ActiveItem(player);

        //    _GameManager.AddCoin((int)itemData.value);
        //}
    }
}