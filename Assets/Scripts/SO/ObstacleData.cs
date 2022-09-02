using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "ScriptableObject/InGame/Obstacle")]
public class ObstacleData : SerializedScriptableObject
{
    [BoxGroup("����")] public EObstacleType type;
    [BoxGroup("����")] public GameObject prefab;

    // bool ��ġ�� int�� ǥ���Ϸ���      idx = (row+1)*(column+1) - 1
    // Random 12�� bool�� ǥ���Ϸ��� arr[rand%6, rand/6]
    [TableMatrix(SquareCells = true)]
    public bool[,] canSpawn = new bool[8, 2];

    [BoxGroup("����")] public EObstacleMoveType moveType;
    [BoxGroup("����")] public float value;

    [BoxGroup("Ǯ��")] public int pool;
}
