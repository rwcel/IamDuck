using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

namespace Floor
{
    public class Bomb : FloorController
    {
        private enum EState
        {
            None,
            Ready,
            Active,
            Disable,
        }

        // **임시 데이터들
        [BoxGroup("색상")]        [ColorPalette] 
        [SerializeField] Color normalColor;
        [BoxGroup("색상")]
        [ColorPalette]
        [SerializeField] Color readyColor;
        [BoxGroup("색상")]        [ColorPalette]
        [SerializeField] Color activeColor;
        [BoxGroup("색상")]        [ColorPalette]
        [SerializeField] Color disableColor;

        [SerializeField] AnimationCurve animationCurve;

        [SerializeField] GameObject activeObj;

        SpriteRenderer sprite;
        bool isReady = false;

        private static readonly float bombMultiple = 1f;

        private void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Reset
        /// </summary>
        protected override void Init()
        {
            ChangeState(EState.None);
        }

        public override void Active(PlayerController player)
        {
            player.SetSpeed();

            if (isReady)
                return;

            ChangeState(EState.Ready);
        }

        IEnumerator CoBomb()
        {
            // 위험한 상태
            ChangeState(EState.Active);

            yield return new WaitForSeconds(data.value * bombMultiple);

            ChangeState(EState.Disable);
            
            // collider hit 판정
        }

        /// <summary>
        /// bool, sprite 변경
        /// </summary>
        /// <param name="state"></param>
        private void ChangeState(EState state)
        {
            switch (state)
            {
                case EState.None:
                    activeObj.SetActive(false);
                    isReady = false;
                    sprite.material.color = normalColor;
                    break;
                case EState.Ready:
                    isReady = true;
                    sprite.material.DOColor(readyColor, data.value)
                        .SetEase(animationCurve)
                        .OnComplete(() =>
                        {
                            if (gameObject.activeInHierarchy)
                                StartCoroutine(nameof(CoBomb));
                        });
                    break;
                case EState.Active:
                    activeObj.SetActive(true);
                    sprite.material.color = activeColor;
                    break;
                case EState.Disable:
                    activeObj.SetActive(false);
                    sprite.material.color = disableColor;
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out PlayerController player))
            {
                if (!player.IsFever)
                {
                    player.CheckIgnoreBomb();
                }
            }
        }
    }
}