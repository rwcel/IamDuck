using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGameItem 
{
    public class Spring : ItemController
    {
        protected override void CheckUpgrade()
        {
            // Ãþ Áõ°¡
            // Debug.Log(itemValue + "," + (int)InGameManager.Instance.UpgradeValue(EUpgradeType.Spring));
            itemValue += (int)InGameManager.Instance.UpgradeValue(EUpgradeType.Spring);
        }

        protected override void Init()
        {
            PlaySequence();
        }

        public override void Active(PlayerController player)
        {
            base.Active(player);

            AudioManager.Instance.PlaySFX(ESfx.Spring);
            player.MoveToSafeZone((int)itemValue);
        }

    }
}
