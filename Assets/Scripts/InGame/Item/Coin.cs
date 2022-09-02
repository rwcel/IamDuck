using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGameItem 
{
    public class Coin : ItemController
    {
        protected override void Init()
        {
            // PlaySequence();
        }

        public override void Active(PlayerController player)
        {
            base.Active(player);

            _GameManager.AddCoin((int)itemValue);
        }
    }
}
