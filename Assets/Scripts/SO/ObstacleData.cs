using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "ScriptableObject/InGame/Obstacle")]
public class ObstacleData : SerializedScriptableObject
{
    [BoxGroup("고유")] public EObstacleType type;
    [BoxGroup("고유")] public GameObject prefab;

    // bool 위치를 int로 표현하려면      idx = (row+1)*(column+1) - 1
    // Random 12를 bool로 표현하려면 arr[rand%6, rand/6]
    [TableMatrix(SquareCells = true)]
    public bool[,] canSpawn = new bool[8, 2];

    [BoxGroup("스탯")] public EObstacleMoveType moveType;
    [BoxGroup("스탯")] public float value;

    [BoxGroup("풀링")] public int pool;
}
