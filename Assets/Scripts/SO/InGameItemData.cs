using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/InGame/Item")]
public class InGameItemData : SerializedScriptableObject
{
    [BoxGroup("고유")] public EIngameItem type;
    [BoxGroup("고유")] public GameObject prefab;

    [BoxGroup("스탯")] public float value;

    [BoxGroup("풀링")] public int pool;

    [BoxGroup("업그레이드")] public UpgradeData upgradeData;
}
