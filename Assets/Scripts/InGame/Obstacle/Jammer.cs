using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Obstacle
{
    public class Jammer : ObstacleController
    {
        // -1 -1.5f
        private static readonly float downRay = 1.5f;
        private static readonly float frontRay = 0.7f;
        private Vector3 dir;

        protected override void Init()
        {
            base.Init();

            dir = Random.Range(0f, 1f) < 0.5f ? Vector3.right: Vector3.left;
            transform.localScale = dir == Vector3.right ? Vector3.one : new Vector3(-1, 1, 1);

            StopCoroutine(nameof(CoMove));
            StartCoroutine(nameof(CoMove));
        }

        private void FixedUpdate()
        {
            transform.Translate(dir * Time.fixedDeltaTime * data.value);
        }

        IEnumerator CoMove()
        {
            while(true)
            {
                yield return Values.Delay01;

                // Debug.DrawRay(transform.position + dir, Vector2.down * downRay, Color.magenta,1f);
                if(!Physics2D.Raycast(transform.position + dir * frontRay, Vector2.down, downRay, Values.Layer_Floor))
                {
                    Turn();
                }
            }
        }

        void Turn()
        {
            dir *= -1;
            transform.localScale = dir == Vector3.right ? Vector3.one : new Vector3(-1, 1, 1);
        }

        /// <summary>
        /// 날아가는 동안 이동 멈추기
        /// </summary>
        /// <param name="target"></param>
        protected override void HitObstacle(Transform target)
        {
            base.HitObstacle(target);

            StopCoroutine(nameof(CoMove));
        }
    }
}
