using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UniRx;

namespace Shop
{
    public class ShopLimitElement : ShopElement
    {
        [BoxGroup("계정제한")]
        [SerializeField] GameObject soldOutObj;

        protected override void Start()
        {
            base.Start();

            // valueText.gameObject.SetActive(false);

            _Server.IsBuyDoubleCoin
                .Subscribe(value => soldOutObj.gameObject.SetActive(value))
                .AddTo(this.gameObject);
        }

        protected override void OnPurchaseComplete()
        {
            base.OnPurchaseComplete();

            // 여러개라면 enum을 부여해서 줘야한다
            _Server.UpdateShopDoubleCoinItem();
        }
    }
}
