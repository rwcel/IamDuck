using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UseItemData", menuName = "ScriptableObject/UseItem Data")]
public class UseItemData : ScriptableObject
{
    [BoxGroup("고유")] public EGameItem id;
    [BoxGroup("고유")] public EUseItemType type;
    [BoxGroup("고유")] public EAds adType;
    // [BoxGroup("고유")] public Sprite icon;

    [BoxGroup("스탯")] public float value;

    [BoxGroup("스탯")] public int cost;               // ...
    [BoxGroup("스탯")] public int maxAdCount;

    [BoxGroup("로컬")] public bool autoEquip;         // 이어하기 티켓은 자동장착

    [BoxGroup("로컬라이징")] public string tableName;
    [BoxGroup("로컬라이징")] public string nameEntry;
    public string valueEntry => nameEntry + " Value";

    // 보유 수 & 시청 광고수는 플레이어 데이터에
}
