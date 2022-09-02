using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Obstacle
{
    public class StaticGear : ObstacleController
    {
      /*  protected override void Init()
        {
            base.Init();

            PlaySequence();
        }

        protected override void PlaySequence()
        {
            base.PlaySequence();

            sequence.Append(transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear))
                .SetLoops(-1, LoopType.Restart);
        }

        //public override void ActiveItem(PlayerController player)
        //{
        //    base.ActiveItem(player);

        //    _GameManager.AddCoin((int)itemData.value);
        //}
      */
    }
}