using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class TypePercent<T>
{
    public T type;
    public float percent;
}

[System.Serializable]
public struct TypeActive<T>
{
    public T Type;
    public int level;
    public int times;  // 배율
}


[System.Serializable]
public struct FLevelFloorInfo
{
    [LabelText("적용 층")]
    public int level;
    [LabelText("개별 확률")]
    public TypePercent<EFloorType>[] typePercents;
}

// **일정 레벨마다 나오게 설정한 데이터 => 폐기
[System.Serializable]
public struct FLevelObstacleInfoToLevel
{
    [LabelText("나올 확률")]
    public TypePercent<int>[] levelPercents;
    public TypeActive<EObstacleType>[] typeActive;
}

[System.Serializable]
public struct FLevelObstacleInfo
{
    [LabelText("적용 층")]
    public int level;
    [LabelText("나올 확률")]
    public float activePercent;
    [LabelText("개별 확률")]
    public TypePercent<EObstacleType>[] typePercents;
}

[System.Serializable]
public struct FLevelItemInfo
{
    [LabelText("적용 층")]
    public int level;
    [LabelText("나올 확률")]
    public float activePercent;
    [LabelText("개별 확률")]
    public TypePercent<EIngameItem>[] typePercents;
}

[System.Serializable]
public struct FHeightRewardInfo
{
    [LabelText("적용 층")]
    public int height;
    [LabelText("보상 코인")]
    public int reward;
}


[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObject/LevelData")]
public class LevelData : SerializedScriptableObject
{
    private const string FileDirectory = "Assets/Resources";
    private const string FilePath = "Assets/Resources/LevelData.asset";
    private static LevelData instance;
    public static LevelData Instance
    {
        get
        {
            if (instance != null)
                return instance;
            instance = Resources.Load<LevelData>("LevelData");
#if UNITY_EDITOR
            if (instance == null)
            {
                if (!AssetDatabase.IsValidFolder(FileDirectory))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                instance = AssetDatabase.LoadAssetAtPath<LevelData>(FilePath);
                if (instance == null)
                {
                    instance = CreateInstance<LevelData>();
                    AssetDatabase.CreateAsset(instance, FilePath);
                }
            }
#endif
            return instance;
        }
    }

    [Title("점수")]
    [PropertySpace(0, 20)]    [LabelText("기록 보상")]
    public int[] HeightRewards;
    //public FHeightRewardInfo[] HeightRewardInfos;

    [TabGroup("Tabs", "InGame")]
    [TabGroup("Tabs/InGame/Group", "발판")]    [PropertySpace(0, 20)]
    public FloorData FirstFloor;
    [TabGroup("Tabs/InGame/Group", "발판")]
    [PropertySpace(0, 20)]
    public int GuideHeight;              // 2,3층만 나오는
    [TabGroup("Tabs/InGame/Group", "발판")]    [PropertySpace(0, 30)]
    public Dictionary<EFloorType, FloorData> FloorMap;
    [TabGroup("Tabs/InGame/Group", "아이템")]    [LabelText("게임 내 아이템")]    [PropertySpace(0, 30)]
    public Dictionary<EIngameItem, InGameItemData> InGameItemMap;
    [TabGroup("Tabs/InGame/Group", "장애물")]    [LabelText("게임 내 장애물")]    [PropertySpace(0, 30)]
    public Dictionary<EObstacleType, ObstacleData> ObstacleMap;

    [TabGroup("Tabs/InGame/Group", "발판")]
    public FLevelFloorInfo[] LevelFloorInfos;
    [TabGroup("Tabs/InGame/Group", "아이템")]
    public FLevelItemInfo[] LevelItemInfos;
    [TabGroup("Tabs/InGame/Group", "장애물")]
    public FLevelObstacleInfo[] LevelObstacleInfos;
    //[TabGroup("Tabs/InGame/Group", "장애물")]
    //public FLevelObstacleInfoToLevel LevelObstacleInfo;

    [TabGroup("Tabs", "OutGame")]
    [TabGroup("Tabs/OutGame/Group", "업그레이드")]
    public UpgradeData[] UpgradeDatas;
    [TabGroup("Tabs/OutGame/Group", "부스터")]
    public UseItemData[] UseItemDatas;

    /// <summary>
    /// 1층부터 시작
    /// </summary>
    public FloorData GetFloorData(int idx, int percent)
    {
        foreach (var floor in LevelFloorInfos[idx].typePercents)
        {
            if (percent < floor.percent)
            {
                return FloorMap[floor.type];
            }
            percent -= (int)floor.percent;
        }

        return null;
    }

    public InGameItemData GetInGameItemData(int idx, float activePercent, int typePercent)
    {
        if (activePercent >= LevelItemInfos[idx].activePercent)
            return null;

        // 아이템 확률 
        foreach (var item in LevelItemInfos[idx].typePercents)
        {
            if (typePercent < item.percent)
            {
                return InGameItemMap[item.type];
            }
            typePercent -= (int)item.percent;
        }

        return null;
    }

    public ObstacleData GetObstacleData(int idx, float activePercent, int typePercent)
    {
        if (activePercent >= LevelObstacleInfos[idx].activePercent)
            return null;

        // 아이템 확률 
        foreach (var item in LevelObstacleInfos[idx].typePercents)
        {
            if (typePercent < item.percent)
            {
                return ObstacleMap[item.type];
            }
            typePercent -= (int)item.percent;
        }

        return null;
    }


    public ObstacleData GetObstacleData(List<EObstacleType> obstacleList, int idx, float activePercent)
    {
        // activePercent -= upgradePercent
        // Debug.Log($"{activePercent} >= {LevelObstacleInfo.levelPercents[idx].percent}");

        //if (activePercent >= LevelObstacleInfo.levelPercents[idx].percent)
            return null;

        int typePercent = Random.Range(0, obstacleList.Count);
        // Debug.Log($"장애물 스폰 : {typePercent} / {obstacleList.Count} => {ObstacleMap[obstacleList[typePercent]]}");

        return ObstacleMap[obstacleList[typePercent]];
    }

    //public int CalcScore(int start, int end)
    //{
    //    int result = 0;
    //    int height = start + 1;         // 0층부터 시작하기때문에 +1

    //    var idx = -1;

    //    foreach (var levelScoreInfo in LevelScoreInfos)
    //    {
    //        if (height < levelScoreInfo.level)
    //            break;
    //        else
    //        {
    //            idx++;
    //            if (height == levelScoreInfo.level)
    //                result += levelScoreInfo.rewardScore;
    //        }
    //    }

    //    do
    //    {
    //        if ( idx + 1 < LevelScoreInfos.Length
    //            && height == LevelScoreInfos[idx + 1].level)
    //            idx++;

    //        result += LevelScoreInfos[idx].score;

    //    } while (height++ < end);

    //    return result;
    //}


    //public void ChangeHeight(int height)
    //{

    //}

}