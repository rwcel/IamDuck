using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UseItemData", menuName = "ScriptableObject/UseItem Data")]
public class UseItemData : ScriptableObject
{
    [BoxGroup("����")] public EGameItem id;
    [BoxGroup("����")] public EUseItemType type;
    [BoxGroup("����")] public EAds adType;
    // [BoxGroup("����")] public Sprite icon;

    [BoxGroup("����")] public float value;

    [BoxGroup("����")] public int cost;               // ...
    [BoxGroup("����")] public int maxAdCount;

    [BoxGroup("����")] public bool autoEquip;         // �̾��ϱ� Ƽ���� �ڵ�����

    [BoxGroup("���ö���¡")] public string tableName;
    [BoxGroup("���ö���¡")] public string nameEntry;
    public string valueEntry => nameEntry + " Value";

    // ���� �� & ��û ������� �÷��̾� �����Ϳ�
}
