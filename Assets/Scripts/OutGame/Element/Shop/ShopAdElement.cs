using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;


namespace Shop
{
    public class ShopAdElement : ShopElement
    {
        [SerializeField] TextMeshProUGUI countText;
        [SerializeField] GameObject cooldownObj;
        private int curCount;

        [SerializeField] TextMeshProUGUI cooldownText;
        // time : DataManager에서 가져오기


        protected override void OnEnable()
        {
            _DataManager.OnAdTimer += UpdateCooldown;
            if (_DataManager.AdTimers[EAds.Coin] > 0)
            {
                cooldownObj.SetActive(true);
                cooldownText.text = _DataManager.AdTimers[EAds.Coin].ValueToTime();     // 초기 데이터
            }

            base.OnEnable();
        }
        private void OnDisable()
        {
            _DataManager.OnAdTimer -= UpdateCooldown;
        }

        void UpdateCooldown(EAds type, int value)
        {
            Debug.Log($"ShopAD : {type} - {value}");

            if (type != EAds.Coin)
                return;

            if(value == 0)
            {
                cooldownObj.SetActive(false);
                cooldownText.text = "";
            }
            else
            {
                cooldownText.text = value.ValueToTime();
            }
        }

        protected override void UpdateData()
        {
            curCount = _DataManager.AdCounts[EAds.Coin];
            countText.text = $"{curCount} / {Values.MAX_Ad_Count}";
        }

        protected override void BuyItem()
        {
            if (cooldownObj.activeSelf || curCount <= 0)
            {
                OnPurchaseFail();
                return;
            }

            UnityAdsManager.Instance.ShowRewardAD(OnPurchaseComplete, OnPurchaseFail, EAds.Coin);
        }

        protected override void OnPurchaseComplete()
        {
            base.OnPurchaseComplete();

            Debug.Log(curCount);

            if (curCount <= 0)
                return;

            _DataManager.AddItem(data.BuyItem);
            _DataManager.SetAdCountAndTime(EAds.Coin);
            cooldownObj.SetActive(true);

            UpdateData();
        }

        protected override void OnPurchaseFail()
        {
            _SystemUI.OpenNoneTouch(Values.Local_Table_Manage, Values.Local_Entry_NotAds);
        }
    }
}
