using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;

namespace Shop
{
    public class ShopBuyElement : ShopElement
    {
        protected override void Awake()
        {
            _DataManager = DataManager.Instance;
            _Server = BackendManager.Instance;
            _SystemUI = SystemUI.Instance;


            int itemValue = 0;
            foreach (var item in data.items)
            {
                itemValue += item.count;
            }

            localizedName = new LocalizedString(Values.Local_Table_Shop, data.entryName)
            {
                {Values.Local_Name_ShopValue,
                    new UnityEngine.Localization.SmartFormat.PersistentVariables.StringVariable
                    {
                        Value = itemValue.CommaThousands()
                    }
                }
            };
        }

        protected override void OnPurchaseComplete()
        {
            base.OnPurchaseComplete();

            foreach (var item in data.items)
            {
                _DataManager.AddItem(item);
            }
        }
    }
}
