using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine.Localization.Settings;

namespace Shop
{
    public class ShopPeriodElement : ShopElement
    {
        [BoxGroup("기간제")]
        [SerializeField] Color normalColor;
        [BoxGroup("기간제")]
        [SerializeField] Color buyColor;
        [BoxGroup("기간제")]
        [SerializeField] TextMeshProUGUI remainText;

        protected override void Start()
        {
            base.Start();

            // valueText.gameObject.SetActive(false);

            _Server.RemainAdRemove
                .Subscribe(value =>
                {
                    var now = System.DateTime.Now;
                    if(value > now)
                    {
                        remainText.text = $"~ {value.ToString("MM.dd")}";
                        remainText.color = buyColor;
                    }
                    else
                    {
                        remainText.color = normalColor;
                    }
                    // Debug.Log(value.Date + "," + value.date);
                    //remainText.color = (value > now) ? buyColor : normalColor;
                })
                .AddTo(this.gameObject);
        }

        protected override void UpdateData()
        {
            base.UpdateData();

            remainText.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                                                Values.Local_Table_Shop, Values.Local_Entry_RemoveAd);
        }

        // void updateData              // 특별한 내용이 없기에 필요 없다고 판단

        protected override void OnPurchaseComplete()
        {
            base.OnPurchaseComplete();

            _Server.UpdateShopRemoveAdItem(data.ItemValue);
        }
    }
}
