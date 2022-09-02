using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;

/// <summary>
/// �ΰ��� �����۵� Ǯ���� �� �����͸� ���� �־��ִ°� ���� �ʳ�?
/// ��ġ�� ������ ������ �����ٵ�
/// ��ȭ��� �����ϸ� �� �𸣱��ϴ�
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
