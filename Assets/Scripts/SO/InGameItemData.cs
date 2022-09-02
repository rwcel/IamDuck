using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/InGame/Item")]
public class InGameItemData : SerializedScriptableObject
{
    [BoxGroup("����")] public EIngameItem type;
    [BoxGroup("����")] public GameObject prefab;

    [BoxGroup("����")] public float value;

    [BoxGroup("Ǯ��")] public int pool;

    [BoxGroup("���׷��̵�")] public UpgradeData upgradeData;
}
