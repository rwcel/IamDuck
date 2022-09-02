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
        [BoxGroup("�Ⱓ��")]
        [SerializeField] Color normalColor;
        [BoxGroup("�Ⱓ��")]
        [SerializeField] Color buyColor;
        [BoxGroup("�Ⱓ��")]
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

        // void updateData              // Ư���� ������ ���⿡ �ʿ� ���ٰ� �Ǵ�

        protected override void OnPurchaseComplete()
        {
            base.OnPurchaseComplete();

            _Server.UpdateShopRemoveAdItem(data.ItemValue);
        }
    }
}
