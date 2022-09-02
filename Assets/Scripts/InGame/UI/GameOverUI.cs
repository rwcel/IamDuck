using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace InGameUIs
{
    public class GameOverUI : PopupUI
    {
        [SerializeField] TextMeshProUGUI heightText;
        [SerializeField] TextMeshProUGUI heightUnitText;
        [SerializeField] TextMeshProUGUI coinText;          // **코인 아이콘을 안불러와도 되는가
        [SerializeField] Color bestColor;
        [SerializeField] ParticleSystem[] recordParticles;

        InGameManager _InGameManager;
        AudioManager _AudioManager;

        private int coinValue;

        private static readonly WaitForSecondsRealtime RealDelay = new WaitForSecondsRealtime(0f);
        private static readonly int _Max_Counting = 40;
        private static readonly int _Anim_Result = Animator.StringToHash("Result");


        protected override void Start()
        {
            base.Start();

            closeButton.onClick.AddListener(ExitGame);
        }

        protected override void UpdateData()
        {
            _InGameManager = InGameManager.Instance;
            _AudioManager = AudioManager.Instance;

            Time.timeScale = 0f;
            _AudioManager.PauseBGM(true);

            float bonusCoin = _DataManager.GetUseItemTypeValue(EUseItemType.BonusCoin);
            if (bonusCoin <= 0)
                bonusCoin = 1;
            bonusCoin *= (BackendManager.Instance.IsBuyDoubleCoin.Value ? 2 : 1);
            // Debug.Log($"코인 보너스 : x{bonusCoin}");

            coinValue = (int)(_InGameManager.Coin.Value * bonusCoin);
            coinText.text = coinValue.ToString();

            StartCoroutine(nameof(CoRaiseHeight));
        }

        IEnumerator CoRaiseHeight()
        {
            int current = 0;
            int goal = _InGameManager.Height.Value;
            int record = _DataManager.Height.Value;
                                // _InGameManager.RecordHeight.Value -> 갱신이 되어있으므로 사용 불가
            bool isFlag = false;
            int perValue = goal / Mathf.Clamp(goal, 1, _Max_Counting);

            _AudioManager.PlayLoopSFX(ESfx.Result);

            while (current <= goal)
            {
                yield return RealDelay;
                current += perValue;
                heightText.text = current.ToString();

                if(current > record && !isFlag)
                {
                    isFlag = true;

                    heightText.color = bestColor;
                    heightUnitText.color = bestColor;

                    foreach (var particle in recordParticles)
                    {
                        particle.Play();
                    }
                }
            }

            _AudioManager.ClearLoop();
            _AudioManager.PlaySFX(ESfx.BestResult);
            heightText.text = goal.ToString();
            anim.SetTrigger(_Anim_Result);
        }

        // *커플링 문제
        void ExitGame()
        {
            _AudioManager.ClearLoop();
            _AudioManager.PauseBGM(false);
            Time.timeScale = 1f;
            _InGameManager.SaveData(coinValue);
            PoolingManager.Instance.AllEnqueueObjects();

            _DataManager.ResetUseItems();
            UnityAdsManager.Instance.ShowInterstitialAD();
            GameSceneManager.Instance.MoveScene(EScene.OutGame, ETransition.Vertical);
        }
    }

}