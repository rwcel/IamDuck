using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Shop
{
    public class ShopExchangeElement : ShopElement
    {
        // Time remainTime;
        // void updateData              // 특별한 내용이 없기에 필요 없다고 판단
        [SerializeField] TextMeshProUGUI valueText;

        protected override void Start()
        {
            base.Start();

            priceText.text = data.price.ToString();
            valueText.text = data.ItemValue.CommaThousands();
        }

        protected override void BuyItem()
        {
            if (_DataManager.UseGoods(EGoods.Dia, data.price))
            {
                OnPurchaseComplete();
            }
            else
            {
                OnPurchaseFail();
            }
        }

        protected override void OnPurchaseComplete()
        {
            base.OnPurchaseComplete();

            _DataManager.AddItem(data.BuyItem);
        }

        protected override void OnPurchaseFail()
        {
            _SystemUI.OpenNoneTouch(Values.Local_Table_Manage, Values.Local_Entry_NeedMoney);
        }
    }
}
