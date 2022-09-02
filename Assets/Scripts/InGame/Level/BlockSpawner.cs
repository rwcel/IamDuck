using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Floor;
using System.Linq;
using static Values;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] WallController leftWall;
    [SerializeField] WallController rightWall;
    [SerializeField] FloorSpawner[] floors;          // 3개 배열
    [SerializeField] Transform obstacleArea;


    // *Type에 따라 구분할 수도 있음
    // *Type.Visible == true, false
    public bool LeftWall => leftWall.gameObject.activeSelf;
    public bool RightWall => rightWall.gameObject.activeSelf;
    public bool LeftFloor => floors[0].gameObject.activeSelf;
    public bool CenterFloor => floors[1].gameObject.activeSelf;
    public bool RightFloor => floors[2].gameObject.activeSelf;

    public int WallCount => LeftWall.GetBoolToInt() + RightWall.GetBoolToInt();
    public int FloorCount => LeftFloor.GetBoolToInt() + CenterFloor.GetBoolToInt() + RightFloor.GetBoolToInt();

    private int height;
    private int rowCount;
    private BlockSpawner belowBlock;
    private GameObject decoObj;

    private int guideHegiht;

    private ObstacleController curObstacle;
    private List<Transform> obstaclePoints;

    public System.Action OnClear;

    PoolingManager _PoolingManager;
    LevelManager _LevelManager;


    private void Awake()
    {
        // Null = First
        if (obstacleArea != null)
        {
            obstaclePoints = new List<Transform>(obstacleArea.childCount);
            foreach (Transform obstaclePoint in obstacleArea)
            {
                obstaclePoints.Add(obstaclePoint);
            }
        }
        else
        {
            foreach (var floor in floors)
            {
                floor.SetFirstFloor();
            }
        }

        rowCount = Length_Obstacle_Row;
        guideHegiht = LevelData.Instance.GuideHeight;

        OnClear += EnqueueFloors;
        OnClear += DestroyDecoObj;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="floor">층 번호</param>
    /// <param name="belowBlock">밑의 층</param>
    public void InitialiseWithData(int _height, BlockSpawner _belowBlock, GameObject decoObj)
    {
        // Debug.Log(_height);

        if (_PoolingManager == null)
            _PoolingManager = PoolingManager.Instance;
        if (_LevelManager == null)
            _LevelManager = LevelManager.Instance;

        height = _height;
        belowBlock = _belowBlock;

        CalcWall();
        CalcFloor();
    }

    private void CalcWall()
    {
        SetWalls(true, true);
        //// 2층 연속으로 나오지 않게만
        //if (belowBlock.WallCount == 0)
        //{
        //    SetWalls(true, true);
        //}
        //else if (!LeftFloor)
        //{
        //    SetWalls(true, RandomWall());
        //}
        //else if (!RightFloor)
        //{
        //    SetWalls(RandomWall(), true);
        //}
        //else
        //{
        //    // 0이상
        //    SetWalls(RandomWall(), RandomWall());
        //}
    }

    /// <summary>
    /// 레벨과 상관없이 생성
    /// </summary>
    private void CalcFloor()
    {
        // Floor
        if (height % AllBlockPerCount == 0)
        {
            bool[] bActive = { true, true, true };
            SetFloors(bActive);
        }
        else if (belowBlock.FloorCount == 1
            || height < guideHegiht)
        {
            // 2이상
            SetFloors(RandomFloor(2));
        }
        else
        {
            // 1이상
            SetFloors(RandomFloor(1));
        }
    }


    private void SetWalls(bool bLeft, bool bRight)
    {
        leftWall.InitialiseWithData(bLeft);
        rightWall.InitialiseWithData(bRight);
    }

    /// <summary>
    /// 바닥은 Object를 만들어줘야함
    /// *Enemy 여기서 만들어 줘야함 -> 바닥 여부에 따라 다르기 때문
    /// </summary>
    private void SetFloors(bool[] bActive)
    {
        FloorData floorData;
        // int obstacleCount = 0;
        // int itemCount = 0;

        for (int i = 0, length = floors.Length; i < length; i++)
        {
            if (bActive[i])
            {
                floorData = _LevelManager.GetFloorData(height);
                floors[i].FloorController = _PoolingManager.Dequeue(floorData.type.ToString(), floors[i].transform)
                                                    .GetComponent<FloorController>();
            }
            else
            {
                floorData = LevelData.Instance.FirstFloor;
                floors[i].FloorController = null;
            }
            floors[i].InitialiseWithData(floorData, height);
        }

        SetObstacle(bActive);
    }

    /// <summary>
    /// 현재 층에 따라 다른 type?
    /// </summary>
    private bool RandomWall()
    {
        // floor
        return Random.Range(0f, 1f) < 0.5;
    }

    private bool[] RandomFloor(int require)
    {
        int count;
        bool[] result = new bool[Length_Floor];
        do
        {
            result[0] = Random.Range(0f, 1f) < 0.5f ? true : false;
            result[1] = Random.Range(0f, 1f) < 0.5f ? true : false;
            result[2] = Random.Range(0f, 1f) < 0.5f ? true : false;

            count = result[0].GetBoolToInt() + result[1].GetBoolToInt() + result[2].GetBoolToInt();

        } while (count < require);

        return result;
    }

    /// <summary>
    /// 층에 있는 오브젝트들 큐에 다시 넣기
    /// </summary>
    public void EnqueueFloors()
    {
        for (int i = 0, length = floors.Length; i < length; i++)
        {
            floors[i].EnqueueObjects();
        }

        if (curObstacle != null)
        {
            _PoolingManager.Enqueue(curObstacle.gameObject, true);
            curObstacle = null;
        }
    }

    private void SetObstacle(bool[] bActives)
    {
        if (bActives.Where(value => value == true).Count() <= 1)
            return;

        var obstacleData = _LevelManager.GetObstacleData(height);
        if (obstacleData == null)
            return;

        int rand = -1;

        if (obstacleData.type == EObstacleType.Jammer)
        {   // bActive 확인
            rand = GetJammerSpawnNum(bActives);
        }
        else
        {
            while (true)
            {
                // Random 16를 bool로 표현하려면 arr[rand%8, rand/8]
                rand = Random.Range(0, obstaclePoints.Count);
                if (obstacleData.canSpawn[rand % rowCount, rand / rowCount])
                    break;
            }
        }

        curObstacle = _PoolingManager.Dequeue(obstacleData.type.ToString(), obstaclePoints[rand])
                            .GetComponent<ObstacleController>();

        curObstacle.InitialiseWithData(obstacleData);
        curObstacle.OnActive = () => curObstacle = null;

    }

    private int GetJammerSpawnNum(bool[] bFloor)
    {
        // Debug.Log($"{bFloor[0]} / {bFloor[1]} / {bFloor[2]}");

        var spawnPoints = new List<int>(6);

        for (int i = 0, length = bFloor.Length; i < length; i++)
        {
            if(bFloor[i])
            {
                spawnPoints.Add(i*2 + Point_Obstacle_InBot);
                spawnPoints.Add((i*2)+1 + Point_Obstacle_InBot);
            }
        }

        return spawnPoints[Utils.RandomNum(spawnPoints.Count)];
    }

    void DestroyDecoObj()
    {
        if (decoObj == null)
            return;
    }
}
