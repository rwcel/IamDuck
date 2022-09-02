using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;


public partial class BackendManager
{
    // SO
    //[SerializeField] MissionData[] missionDatas;
    //[SerializeField] MissionRewardData[] missionRewardDatas;

    [SerializeField] bool isSave;

    private void GetChartLists()
    {
        var bro = Backend.Chart.GetChartList();
        if (FailError(bro, "차트 불러오기"))
            return;

        var json = bro.FlattenRows();

        foreach (JsonData chart in json)
        {
            GetChart
                (
                    (EChart)System.Enum.Parse(typeof(EChart), chart["chartName"].ToString()),
                    chart["selectedChartFileId"].ToString()
                );
        }
    }

    /// <summary>
    /// fileId 값이 달라지면 서버에서 가져오기
    /// </summary>
    void GetChart(EChart type, string fileId)
    {
        if(isSave)
        {
            GetChartToServer(type, fileId);
            return;
        }

        var data = Backend.Chart.GetLocalChartData(fileId);
        if (data == "")
        {
            SaveChart(type, fileId);
        }
        else
        {
            SetChartData(type, data);
        }
    }

    void SaveChart(EChart type, string key)
    {
        Backend.Chart.GetOneChartAndSave(key, bro =>
        {
            if (!bro.IsSuccess())
                return;

            JsonData rows = bro.FlattenRows();
            SetFlattenChartData(type, rows);
        });
    }

    void GetChartToServer(EChart type, string key)
    {
        Backend.Chart.GetChartContents(key, bro =>
        {
            if (!bro.IsSuccess())
                return;

            JsonData rows = bro.FlattenRows();
            SetFlattenChartData(type, rows);
        });
    }

    void SetChartData(EChart type, string data)
    {
        var bro = JsonMapper.ToObject(data);
        var rows = bro["rows"];

        switch (type)
        {
            case EChart.Attendance:
                SetAttendanceData(rows, false);
                break;
            case EChart.DailyMission:
                SetDailyMissionData(rows, false);
                break;
            case EChart.MissionReward:
                SetMissionRewardData(rows, false);
                break;
            case EChart.Character:
                SetCharacterData(rows, false);
                break;
            case EChart.Shop:
                SetShopData(rows, false);
                break;
            default:
                Debug.LogWarning("없는 차트가 있음 : " + type);
                break;
        }
    }

    void SetFlattenChartData(EChart type, JsonData rows)
    {
        switch (type)
        {
            case EChart.Attendance:
                SetAttendanceData(rows, true);
                break;
            case EChart.DailyMission:
                SetDailyMissionData(rows, true);
                break;
            case EChart.MissionReward:
                SetMissionRewardData(rows, true);
                break;
            case EChart.Character:
                SetCharacterData(rows, true);
                break;
            case EChart.Shop:
                SetShopData(rows, true);
                break;
            default:
                Debug.LogWarning("없는 차트가 있음 : " + type);
                break;
        }
    }

    void SetAttendanceData(JsonData rows, bool isFlatten)
    {
        var attendanceDatas = new FDailyCheck[rows.Count];

        if(isFlatten)
        {
            for (int i = 0, length = rows.Count; i < length; i++)
            {
                attendanceDatas[i].day = int.Parse(rows[i]["day"].ToString());
                attendanceDatas[i].id = int.Parse(rows[i]["rewardID"].ToString());
                attendanceDatas[i].value = int.Parse(rows[i]["value"].ToString());
            }
        }
        else
        {
            for (int i = 0, length = rows.Count; i < length; i++)
            {
                attendanceDatas[i].day = int.Parse(rows[i]["day"]["S"].ToString());
                attendanceDatas[i].id = int.Parse(rows[i]["rewardID"]["S"].ToString());
                attendanceDatas[i].value = int.Parse(rows[i]["value"]["S"].ToString());
            }
        }

        _DataManager.InitializeAttendance(attendanceDatas);
    }

    private void SetDailyMissionData(JsonData rows, bool isFlatten)
    {
        var missionDatas = _GameData.MissionDatas;

        if(isFlatten)
        {
            for (int i = 0, length = rows.Count; i < length; i++)
            {
                missionDatas[i].num = int.Parse(rows[i]["num"].ToString());
                missionDatas[i].category = (EMissionCategory)int.Parse(rows[i]["category"].ToString());
                missionDatas[i].type = int.Parse(rows[i]["type"].ToString());
                missionDatas[i].typeBit = int.Parse(rows[i]["typeBit"].ToString());
                missionDatas[i].missionValue = int.Parse(rows[i]["missionValue"].ToString());
                missionDatas[i].isBar = rows[i]["isBar"].ToString() == "TRUE";
                missionDatas[i].gauge = int.Parse(rows[i]["gauge"].ToString());
                missionDatas[i].rewardID = int.Parse(rows[i]["rewardID"].ToString());
                missionDatas[i].rewardValue = int.Parse(rows[i]["rewardValue"].ToString());
            }
        }
        else
        {
            for (int i = 0, length = rows.Count; i < length; i++)
            {
                missionDatas[i].num = int.Parse(rows[i]["num"]["S"].ToString());
                missionDatas[i].category = (EMissionCategory)int.Parse(rows[i]["category"]["S"].ToString());
                missionDatas[i].type = int.Parse(rows[i]["type"]["S"].ToString());
                missionDatas[i].typeBit = int.Parse(rows[i]["typeBit"]["S"].ToString());
                missionDatas[i].missionValue = int.Parse(rows[i]["missionValue"]["S"].ToString());
                missionDatas[i].isBar = rows[i]["isBar"]["S"].ToString() == "TRUE";
                missionDatas[i].gauge = int.Parse(rows[i]["gauge"]["S"].ToString());
                missionDatas[i].rewardID = int.Parse(rows[i]["rewardID"]["S"].ToString());
                missionDatas[i].rewardValue = int.Parse(rows[i]["rewardValue"]["S"].ToString());
            }
        }

        _DataManager.InitializeDailyMission(missionDatas);
    }

    private void SetMissionRewardData(JsonData rows, bool isFlatten)
    {
        var missionRewardDatas = _GameData.MissionRewardDatas;

        if (isFlatten)
        {
            for (int i = 0, length = rows.Count; i < length; i++)
            {
                missionRewardDatas[i].num = i;
                missionRewardDatas[i].gauge = int.Parse(rows[i]["gauge"].ToString());
                missionRewardDatas[i].id = int.Parse(rows[i]["itemID"].ToString());
                missionRewardDatas[i].value = int.Parse(rows[i]["value"].ToString());
            }
        }
        else
        {
            for (int i = 0, length = rows.Count; i < length; i++)
            {
                missionRewardDatas[i].num = i;
                missionRewardDatas[i].gauge = int.Parse(rows[i]["gauge"]["S"].ToString());
                missionRewardDatas[i].id = int.Parse(rows[i]["itemID"]["S"].ToString());
                missionRewardDatas[i].value = int.Parse(rows[i]["value"]["S"].ToString());
            }
        }

        _DataManager.InitializeMissionReward(missionRewardDatas);
    }

    private void SetCharacterData(JsonData rows, bool isFlatten)
    {
        var unlockCharacters = _GameData.CharacterDatas;
        //for (int i = 0, length = rows.Count; i < length; i++)

        if (isFlatten)
        {
            for (int i = 0, length = unlockCharacters.Length; i < length; i++)
            {
                unlockCharacters[i].id = (EGameItem)int.Parse(rows[i]["itemID"].ToString());
                unlockCharacters[i].charID = (ECharacter)(unlockCharacters[i].id - EGameItem.NormalChar);
                //unlockCharacters[i].titleName = rows[i]["name"].ToString();
                //unlockCharacters[i].descName = rows[i]["desc"].ToString();
                unlockCharacters[i].category = (ECharacterCategory)int.Parse(rows[i]["category"].ToString());
                unlockCharacters[i].type = int.Parse(rows[i]["type"].ToString());
                unlockCharacters[i].value = int.Parse(rows[i]["value"].ToString());
                unlockCharacters[i].unlockEntry = $"Unlock {int.Parse(rows[i]["unlockEntry"].ToString())}";
            }
        }
        else
        {
            for (int i = 0, length = unlockCharacters.Length; i < length; i++)
            {
                unlockCharacters[i].id = (EGameItem)int.Parse(rows[i]["itemID"]["S"].ToString());
                unlockCharacters[i].charID = (ECharacter)(unlockCharacters[i].id - EGameItem.NormalChar);
                //unlockCharacters[i].titleName = rows[i]["name"]["S"].ToString();
                //unlockCharacters[i].descName = rows[i]["desc"]["S"].ToString();
                unlockCharacters[i].category = (ECharacterCategory)int.Parse(rows[i]["category"]["S"].ToString());
                unlockCharacters[i].type = int.Parse(rows[i]["type"]["S"].ToString());
                unlockCharacters[i].value = int.Parse(rows[i]["value"]["S"].ToString());
                unlockCharacters[i].unlockEntry = $"Unlock {int.Parse(rows[i]["unlockEntry"]["S"].ToString())}";
            }
        }

        // SO 적용
    }

    private void SetShopData(JsonData rows, bool isFlatten) 
    {
        var shopDatas = _GameData.ShopDatas;

        // *type대로 맞추기 위해 GameData의 순서를 똑같이 해야함
        if (isFlatten)
        {
            for (int i = 0, length = shopDatas.Length; i < length; i++)
            {
                shopDatas[i].type = (EShopType)System.Enum.Parse(typeof(EShopType), rows[i]["type"].ToString());
                shopDatas[i].price = int.Parse(rows[i]["price"].ToString());
                FGetItem[] shopItem = new FGetItem[int.Parse(rows[i]["itemCount"].ToString())];
                shopDatas[i].items = shopItem;
                for (int j = 0, length2 = shopItem.Length; j < length2; j++)
                {
                    shopDatas[i].items[j].type = (EGameItem)int.Parse(rows[i][$"item{j + 1}ID"].ToString());
                    shopDatas[i].items[j].count = int.Parse(rows[i][$"item{j + 1}Value"].ToString());
                }
                //shopDatas[i].title = rows[i]["name"].ToString();
                //shopDatas[i].content = rows[i]["desc"].ToString();
                shopDatas[i].salePercent = int.Parse(rows[i]["salePercent"].ToString());
            }
        }
        else
        {
            for (int i = 0, length = shopDatas.Length; i < length; i++)
            {
                shopDatas[i].type = (EShopType)System.Enum.Parse(typeof(EShopType), rows[i]["type"]["S"].ToString());
                shopDatas[i].price = int.Parse(rows[i]["price"]["S"].ToString());
                FGetItem[] shopItem = new FGetItem[int.Parse(rows[i]["itemCount"]["S"].ToString())];
                shopDatas[i].items = shopItem;
                for (int j = 0, length2 = shopItem.Length; j < length2; j++)
                {
                    shopDatas[i].items[j].type = (EGameItem)int.Parse(rows[i][$"item{j + 1}ID"]["S"].ToString());
                    shopDatas[i].items[j].count = int.Parse(rows[i][$"item{j + 1}Value"]["S"].ToString());
                }

                //shopDatas[i].title = rows[i]["name"]["S"].ToString();
                //shopDatas[i].content = rows[i]["desc"]["S"].ToString();
                shopDatas[i].salePercent = int.Parse(rows[i]["salePercent"]["S"].ToString());
            }
        }


        OnestoreManager.Instance.ForceConsumeProduct();
    }
}
