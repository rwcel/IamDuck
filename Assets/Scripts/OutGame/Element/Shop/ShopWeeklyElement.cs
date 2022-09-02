using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UniRx;

namespace Shop
{
    public class ShopWeeklyElement : ShopElement
    {
        [BoxGroup("�ְ�")]
        [SerializeField] Image[] itemImages;
        [BoxGroup("�ְ�")]
        [SerializeField] TextMeshProUGUI[] valueTexts;
        [BoxGroup("�ְ�")]
        [SerializeField] GameObject soldOutObj;

        // void updateData              // Ư���� ������ ���⿡ �ʿ� ���ٰ� �Ǵ�

        protected override void Start()
        {
            base.Start();

            for (int i = 0, length = data.items.Length; i < length; i++)
            {
                itemImages[i].sprite = GameData.Instance.GameItemSpriteMap[data.items[i].type];
                valueTexts[i].text = data.items[i].count.CommaThousands();
            }

            _Server.BuyWeeklyTime
                .Subscribe(value =>
                {
                    soldOutObj.gameObject.SetActive(value > System.DateTime.Now);
                })
                .AddTo(this.gameObject);

        }

        protected override void UpdateData()
        {
            base.UpdateData();

        }

        protected override void OnPurchaseComplete()
        {
            base.OnPurchaseComplete();

            // items add
            foreach (var item in data.items)
            {
                _DataManager.AddItem(item);
            }

            _Server.UpdateShopWeeklyItem();
        }
    }
}
