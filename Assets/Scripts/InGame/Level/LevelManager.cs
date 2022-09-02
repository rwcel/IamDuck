using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] GameObject blockPrefab;
    [SerializeField] BlockSpawner firstBlock;
    [SerializeField] Transform floorParent;
    [SerializeField] int floorCount;      // 풀링 개수

    [SerializeField] FloorDeco bestHeightDeco;
    [SerializeField] FloorDeco heightRewardDeco;

    // [SerializeField] Transform wallColliderTr;

    private BlockSpawner belowBlock;
    private int moveFloorNum;           // 풀링 이동 개수 => floorCount / 3;

    Queue<BlockSpawner> blocks = new Queue<BlockSpawner>();

    LevelData _LevelData;
    InGameManager _InGameManager;

    private int levelFloorIdx = 0, levelItemIdx = 0, levelObstacleIdx = 0;
    private int nextfloorLevel = 0, nextItemLevel = 0, nextObstacleLevel = 0;
    //List<EObstacleType> obstacleList;

    private float itemApearPercent;
    private float obstacleAppearPercent;


    protected override void AwakeInstance()
    {
        blocks = new Queue<BlockSpawner>();

        // obstacleList = new List<EObstacleType>(100);            // 임시 공간
    }

    protected override void DestroyInstance()
    {
    }

    private void Start()
    {
        _LevelData = LevelData.Instance;
        _InGameManager = InGameManager.Instance;

        itemApearPercent = _InGameManager.UpgradeValue(EUpgradeType.ItemAppear);
        obstacleAppearPercent = DataManager.Instance.GetUseItemTypeValue(EUseItemType.ObstacleAppear);

        SetNextLevels();

        CreateBlocks();

        moveFloorNum = floorCount / 3;

        _InGameManager.Height
            .Where(y => y % moveFloorNum == 0 && y != moveFloorNum && y != 0)
            .Subscribe(y => MoveNewBlocks(y))
            .AddTo(this.gameObject);

        //InGameManager.Instance.Height
        //    .Subscribe(value => UpdateHeight(value));
    }

    #region 블럭

    void CreateBlocks()
    {
        belowBlock = firstBlock;

        var playerFloor = 0;            // 인수 받기

        // 0층은 기본적으로 생성되어 있음
        for (int i = playerFloor + 1; i <= floorCount; i++)
        {
            // 계산식 세우기
            var block = Instantiate(blockPrefab, Vector2.up * (Values.FloorPerY * i), Quaternion.identity, floorParent)
                            .GetComponent<BlockSpawner>();

            SettingBlock(block, i);
        }
    }

    /// <summary>
    /// 큐의 일정개수들을 빼서 위로 올리고 다시 큐에 넣기
    /// </summary>
    /// <param name="height"></param>
    void MoveNewBlocks(int height)
    {
        for (int i = 1, length = moveFloorNum; i <= length; i++)
        {
            var block = blocks.Dequeue();
            block.transform.position += new Vector3(0, floorCount * Values.FloorPerY, 0);
            block.OnClear?.Invoke();

            SettingBlock(block, height + moveFloorNum + i);
        }
    }

    private int curRewardHeightIdx = 0;

    void SettingBlock(BlockSpawner block, int height)
    {
        GameObject decoObj = null;
        FloorDeco deco = null;

        // Calc Deco
        if(height == _InGameManager.RecordHeight.Value)
        {
            decoObj = bestHeightDeco.gameObject;
            deco = bestHeightDeco;
        }
        else if(curRewardHeightIdx < _LevelData.HeightRewards.Length
            && height == _LevelData.HeightRewards[curRewardHeightIdx])
        {
            curRewardHeightIdx++;
            heightRewardDeco.SetText(height.ToString());
            decoObj = heightRewardDeco.gameObject;
            deco = heightRewardDeco;
        }

        // Debug.Log($"{height} == {_LevelData.HeightRewards[curRewardHeightIdx]}");

        if(decoObj != null)
        {
            deco.SetHeight(height);
            decoObj.transform.SetParent(block.transform, false);
            decoObj.SetActive(true);
            block.OnClear += () =>
            {
                if(decoObj == null)
                    return;
                Debug.Log("Deco Clear");
                decoObj.transform.SetParent(transform, false);
                decoObj.SetActive(false);
                decoObj = null;
            };
        }

        block.InitialiseWithData(height, belowBlock, decoObj);

        blocks.Enqueue(block);
        belowBlock = block;
    }

    ///// <summary>
    ///// Best : height == bestHeight
    ///// HeightRewards : data == highReward[curReward];
    ///// </summary>
    //private void CalcDeco()
    //{
    //    // Best
    //    if (height == _InGameManger.RecordHeight.Value)
    //    {
    //        // 반납 필요
    //        SpawnBestObj();
    //        return;
    //    }

    //    // HeightRewards
    //}

    //void MoveWallColliders()
    //{
    //    wallColliderTr.position += Vector3.up * Values.FloorPerY * moveFloorNum;
    //}

    #endregion

    #region 레벨

    void SetNextLevels()
    {
        NextFloorLevel(0);
        NextItemLevel(0);
        //NextObstacleLevel(0, 0);
        NextObstacleLevel(0);
    }

    void NextFloorLevel(int value = 1)
    {
        levelFloorIdx += value;
        nextfloorLevel = (levelFloorIdx + 1 < _LevelData.LevelFloorInfos.Length) ?
                            _LevelData.LevelFloorInfos[levelFloorIdx + 1].level : -1;
    }

    void NextItemLevel(int value = 1)
    {
        levelItemIdx += value;
        nextItemLevel = (levelItemIdx + 1 < _LevelData.LevelItemInfos.Length) ?
                            _LevelData.LevelItemInfos[levelItemIdx + 1].level : -1;
    }

    void NextObstacleLevel(int value = 1)
    {
        levelObstacleIdx += value;
        nextObstacleLevel = (levelObstacleIdx + 1 < _LevelData.LevelObstacleInfos.Length) ?
                    _LevelData.LevelObstacleInfos[levelObstacleIdx + 1].level : -1;
    }

    /// <summary>
    /// * 꼭 그 높이부터 생성되는 조건이여야 작동
    /// </summary>
    void NextObstacleLevel(int height, int value = 1)
    {
        return;

        //var obstacleInfo = _LevelData.LevelObstacleInfo;

        //levelObstacleIdx += value;
        //nextObstacleLevel = (levelObstacleIdx + 1 < obstacleInfo.levelPercents.Length) ?
        //            obstacleInfo.levelPercents[levelObstacleIdx + 1].type : -1;

        //foreach (var obstacle in obstacleInfo.typeActive)
        //{
        //    if (height == obstacle.level)
        //    {
        //        for (int i = 0, length = obstacle.times; i < length; i++)
        //        {
        //            obstacleList.Add(obstacle.Type);
        //        }
        //    }
        //}
    }

    public FloorData GetFloorData(int height)
    {
        if ((nextfloorLevel != -1) && (height >= nextfloorLevel))
        {
            NextFloorLevel();
        }

        return _LevelData.GetFloorData(levelFloorIdx, Utils.RandomNum(100));
    }

    public InGameItemData GetItemData(int height)
    {
        if ((nextItemLevel != -1) && (height >= nextItemLevel))
        {
            NextItemLevel();
        }

        return _LevelData.GetInGameItemData(levelItemIdx, Utils.RandomNum(100) + itemApearPercent, Utils.RandomNum(100));
    }

    /// <summary>
    /// ** => item Percent
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    public ObstacleData GetObstacleData(int height)
    {
        if ((nextObstacleLevel != -1) && (height >= nextObstacleLevel))
        {
            NextObstacleLevel();
        }

        return _LevelData.GetObstacleData(levelObstacleIdx, Utils.RandomNum(100) - obstacleAppearPercent, Utils.RandomNum(100));
        // return _LevelData.GetObstacleData(obstacleList, levelObstacleIdx, Utils.RandomNum(100) - obstacleAppearPercent);
    }

    #endregion
}
