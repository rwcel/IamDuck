using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Floor
{
    public abstract class FloorController : MonoBehaviour
    {
        private EFloorType type;
        public EFloorType Type => type;

        protected InGameManager _GameManager;
        protected PoolingManager _PoolingManager;

        protected FloorData data;

        private void Start()
        {
            _GameManager = InGameManager.Instance;
            _PoolingManager = PoolingManager.Instance;
        }

        public void InitialiseWithData(FloorData data)
        {
            this.data = data;
            type = data.type;

            Init();
        }

        protected abstract void Init();
        public abstract void Active(PlayerController player);

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (collision.TryGetComponent(out PlayerController player))
        //    {
        //        player.ChangeState(EPlayerState.Hit);
        //    }
        //}
    }
}