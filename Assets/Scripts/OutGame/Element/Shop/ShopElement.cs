using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;

namespace Shop
{
    public abstract class ShopElement : MonoBehaviour
    {
        [SerializeField] protected Button buyButton;

        [SerializeField] protected ShopData data;

        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] protected TextMeshProUGUI priceText;
        [SerializeField] Image image;

        protected DataManager _DataManager;
        protected BackendManager _Server;
        protected SystemUI _SystemUI;

        private bool isTestMode;
        protected EStore store;

        protected LocalizedString localizedName;


        protected virtual void Awake()
        {
            _DataManager = DataManager.Instance;
            _Server = BackendManager.Instance;
            _SystemUI = SystemUI.Instance;

            localizedName = new LocalizedString(Values.Local_Table_Shop, data.entryName)
            {
                {Values.Local_Name_ShopValue,
                    new UnityEngine.Localization.SmartFormat.PersistentVariables.StringVariable{
                        Value = data.ItemValue.CommaThousands()}
                    }
            };
        }

        protected virtual void OnEnable()
        {
            nameText.text = localizedName.GetLocalizedString();

            UpdateData();
        }

        protected virtual void Start()
        {
            store = GameData.Instance.Store;
            //isTestMode = GameApplication.Instance.IsTestMode;

            if (priceText != null && data.productID != "")
            {
#if UNITY_EDITOR
                priceText.text = $"\\{data.price.CommaThousands()}";                 // 시스템 상이기때문에 onenable 아님
#else
                priceText.text = IAPManager.Instance.GetPrice(data.productID);
#endif
            }

            Debug.Assert(buyButton != null, $"사는 버튼이 없습니다. : {gameObject.name}");
            buyButton.onClick.AddListener(OnBuy);
        }

        protected virtual void UpdateData() { }

        private void OnBuy()
        {
            BuyItem();
        }

        protected virtual void BuyItem()
        {
            if (data.productID == "")
                return;

            switch (store)
            {
                case EStore.Google:
                    IAPManager.Instance.BuyProductID(data.productID, OnPurchaseComplete, OnPurchaseFail);
                    break;
                case EStore.Onestore:
                    OnestoreManager.Instance.BuyProductID(data.productID, OnPurchaseComplete, OnPurchaseFail);
                    break;
                default:
                    Debug.LogWarning("스토어 없음");
                    break;
            }
        }
        protected virtual void OnPurchaseComplete()
        {
            AudioManager.Instance.PlaySFX(ESfx.Reward);
            _SystemUI.OpenNoneTouch(Values.Local_Table_Shop, Values.Local_Entry_PurchaseSuccess);

            if (data.price > 0)
            {
                _Server.UpdatePayment(data.price);
            }
        }
        protected virtual void OnPurchaseFail()
        {
            _SystemUI.OpenNoneTouch(Values.Local_Table_Shop, Values.Local_Entry_PurchaseFail);
        }

    }
}