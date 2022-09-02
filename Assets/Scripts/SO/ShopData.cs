using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FGetItem
{
    public EGameItem type;          // or id
    public int count;

    public FGetItem(EGameItem type, int count)
    {
        this.type = type;
        this.count = count;
    }
}

//[CreateAssetMenu(fileName = "ShopData", menuName = "ScriptableObject/InGame/Floor")]
[CreateAssetMenu(menuName = "ScriptableObject/Lobby/Shop")]
public class ShopData : ScriptableObject
{
    public EShopType type;
    public int price;
    public Sprite iconSprite;
    public FGetItem[] items;           // itemCount

    public string productID;

    //public string title;
    //public string content;
    public int salePercent;
    public string entryName;

    // 0번은 모두 가지고 있음
    public EGameItem ItemId => items[0].type;
    public int ItemValue => items[0].count;

    public FGetItem BuyItem => items[0];
}
