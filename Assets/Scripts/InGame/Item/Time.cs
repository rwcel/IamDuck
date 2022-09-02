using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InGameItem
{
    public class Time : ItemController
    {
        protected override void Init()
        {

        }

        public override void Active(PlayerController player)
        {
            base.Active(player);

            // _GameManager.AddTime((int)itemData.value);
        }
    }
}
