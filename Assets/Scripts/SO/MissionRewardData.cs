using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionRewardData", menuName = "ScriptableObject/Mission/Reward")]
public class MissionRewardData : ScriptableObject
{
    [BoxGroup("����")] public int num;        // idx. ���� ��� ����
    [BoxGroup("����")] public int gauge;

    [BoxGroup("����")] public int id;
    [BoxGroup("����")] public int value;
}
