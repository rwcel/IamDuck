using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGameItem
{
    public class Potion : ItemController
    {
        protected override void CheckUpgrade()
        {
            // Ãþ Áõ°¡
            itemValue += InGameManager.Instance.UpgradeValue(EUpgradeType.PowerPotion);
        }

        protected override void Init()
        {

        }

        public override void Active(PlayerController player)
        {
            base.Active(player);

            AudioManager.Instance.PlaySFX(ESfx.PowerPotion);
            player.Fever(itemValue);
        }
    }
}
