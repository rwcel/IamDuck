using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "FloorData", menuName = "ScriptableObject/InGame/Floor")]
public class FloorData : SerializedScriptableObject
{
    [BoxGroup("����")] public EFloorType type;
    [BoxGroup("����")] public GameObject prefab;

    [BoxGroup("����")] public float value;

    [BoxGroup("Ǯ��")] public int pool;

}