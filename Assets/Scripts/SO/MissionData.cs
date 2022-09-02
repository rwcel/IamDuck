using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "ScriptableObject/Mission/Mission")]
public class MissionData : ScriptableObject
{
    [BoxGroup("고유")] public int num;                // idx
    public int Idx =>  num;
    [BoxGroup("고유")] public int missionValue;
    [BoxGroup("고유")] public bool isBar;

    [BoxGroup("보상")] public int gauge;
    [BoxGroup("보상")] public int rewardID;
    [BoxGroup("보상")] public int rewardValue;

    [BoxGroup("임시")] public string nameText;
    [BoxGroup("로컬라이징")] public string tableName;
    [BoxGroup("로컬라이징")] public string keyName;

    [HideInInspector] public EMissionCategory category;
    [HideInInspector] public int type;
    [HideInInspector] public int typeBit;

    public int currentValue;

    public System.Action<int, int> OnAchieveProgress;
    public System.Action OnAchieveRecv;


    public void SetData(System.Action<int, int> action)
    {
        OnAchieveProgress = action;
    }

    public virtual void OnAchieve(int type, int value)
    {
        if (typeBit != -1)
        {
            // bitTypeCheck
            // Debug.Log($"{1 << type} & {typeBit} => {1 << type & typeBit}");
            if ((1 << type & typeBit) != 0)
            {
                OnAchieveProgress?.Invoke(num, value);
            }
        }
        else
        {   // typeCheck
            if (this.type == type)
            {
                Debug.Log($"OnAchieveProgress : {num} - {value}");
                OnAchieveProgress?.Invoke(num, value);
            }
        }
    }

}