using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Floor
{
    public class Swamp : FloorController
    {
        protected override void Init()
        {

        }

        public override void Active(PlayerController player)
        {
            player.SetSpeed(data.value);
        }
    }
}