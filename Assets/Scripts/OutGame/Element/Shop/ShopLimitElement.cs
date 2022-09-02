using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UniRx;

namespace Shop
{
    public class ShopLimitElement : ShopElement
    {
        [BoxGroup("��������")]
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

            // ��������� enum�� �ο��ؼ� ����Ѵ�
            _Server.UpdateShopDoubleCoinItem();
        }
    }
}
