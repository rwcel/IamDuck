using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "ScriptableObject/Upgrade Data")]
public class UpgradeData : ScriptableObject
{
    [BoxGroup("����")] public EGameItem id;
    [BoxGroup("����")] public EUpgradeType type;
    // [BoxGroup("����")] public Sprite icon;

    [BoxGroup("����")] public int baseCost;
    [BoxGroup("����")] public float upgradeValue;
    [BoxGroup("����")] public int maxLevel;

    [BoxGroup("������")] public InGameItemData itemData;

    [BoxGroup("���ö���¡")] public string tableName;
    [BoxGroup("���ö���¡")] public string nameEntry;
    public string valueEntry => nameEntry + " Value";


    // ���� ���൵�� �÷��̾� �����Ϳ�

    public string GetUpgradeText(int level)
    {
        var result = GetUpgradeValue(level);

        if(type == EUpgradeType.Spring)
            return $"{result - Values.Dist_SpringY} ~ {result}";
        else
            return $"{Mathf.RoundToInt(result * 100) / 100.0f}";

        //switch (type)
        //{
        //    case EUpgradeType.PowerPotion:
        //    case EUpgradeType.Magnet:
        //        return $"{result}s";
        //    case EUpgradeType.Spring:
        //        return $"{result - Values.Dist_SpringY} ~ {result}";
        //    case EUpgradeType.ItemAppear:
        //        return $"{result}%";
        //    case EUpgradeType.Resist:
        //        return $"{result}%";
        //}

        //return "";
    }

    public float GetUpgradeValue(int level)
    {
        if (itemData == null)
            return upgradeValue * level;

        return itemData.value + upgradeValue * level;
    }

    public int GetCost(int level)
    {
        if (level >= maxLevel)
            return -1;

        switch (type)
        {
            case EUpgradeType.PowerPotion:
            case EUpgradeType.Magnet:
            case EUpgradeType.Resist:
                return FuncB(level);
            case EUpgradeType.Spring:
                return FuncC(level);
            case EUpgradeType.ItemAppear:
                return FuncA(level);
        }

        return -1;
    }

    /// <summary>
    /// ��ֹ� ���� Ȯ�� ����
    /// +300 -> *2 -> *2 ->  -> 
    /// </summary>
    public int FuncA(int level)
    {
        int cost = baseCost;

        cost *= (int)Mathf.Pow(2, level / 10);

        return cost;
    }

    /// <summary>
    /// �Ŀ�����, �ڼ�
    /// +100 -> +200 -> 300 -> 400 -> (+500 -> ...)
    /// </summary>
    public int FuncB(int level)
    {
        int cost = baseCost;
        int costMul = 0;
        int idx = 0;
        int max = Mathf.Clamp(level / 5 + 1, 0, maxLevel);
        while (idx++ < max)
        {
            if (costMul < 500)
                costMul += 100;
            cost += costMul;
        }

        return cost;
    }

    /// <summary>
    /// �ٶ�����
    /// 500 (*2 -> *5 -> ...)
    /// </summary>
    public int FuncC(int level)
    {
        int cost = baseCost;
        for (int i = 0; i <= level; i++)
        {
            cost *= (i % 2 == 1) ? 5 : 2;
        }

        return cost;
    }

}
