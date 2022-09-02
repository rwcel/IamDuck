using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;

/// <summary>
/// 인게임 아이템들 풀링할 때 데이터를 직접 넣어주는게 낫지 않나?
/// 수치가 어차피 변하지 않을텐데
/// 강화요소 생각하면 또 모르긴하다
/// </summary>
namespace InGameItem 
{
    public class Magnet : ItemController
    {
        protected override void CheckUpgrade()
        {
            // 
            itemValue += InGameManager.Instance.UpgradeValue(EUpgradeType.Magnet);
        }

        protected override void Init()
        {
            // PlaySequence();
        }

        public override void Active(PlayerController player)
        {
            base.Active(player);

            AudioManager.Instance.PlaySFX(ESfx.Magnet);
            player.MagnetCollider.Active(itemValue);
        }
    }
}
