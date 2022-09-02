using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "FloorData", menuName = "ScriptableObject/InGame/Floor")]
public class FloorData : SerializedScriptableObject
{
    [BoxGroup("고유")] public EFloorType type;
    [BoxGroup("고유")] public GameObject prefab;

    [BoxGroup("스탯")] public float value;

    [BoxGroup("풀링")] public int pool;

}