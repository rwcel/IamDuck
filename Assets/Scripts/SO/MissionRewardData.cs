using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionRewardData", menuName = "ScriptableObject/Mission/Reward")]
public class MissionRewardData : ScriptableObject
{
    [BoxGroup("고유")] public int num;        // idx. 서버 사용 안함
    [BoxGroup("고유")] public int gauge;

    [BoxGroup("보상")] public int id;
    [BoxGroup("보상")] public int value;
}
