using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankUI : PopupUI
{
    [SerializeField] Transform listParent;
    [SerializeField] GameObject elementPrefab;

    [SerializeField] RankElement myElement;
    [SerializeField] RankElement rankInMyElement;       // 랭킹 내에 있는 나의 정보

    private bool isUpdate;
    private int elementCount;

    [SerializeField] GameObject[] topTierObjs;
    BackendManager _Server;

    private List<RankElement> rankElements = new List<RankElement>(100);
    int rankCount = 0;

    int myRank;

    protected override void Awake()
    {
        base.Awake();

        _Server = BackendManager.Instance;

        foreach (Transform item in listParent)
        {
            var element = item.GetComponent<RankElement>();
            rankElements.Add(element);
        }
    }

    protected override void UpdateData()
    {
        if (isUpdate)
        {
            UpdateMyDatas();
            UpdateElementDatas();
            return;
        }

        isUpdate = true;
        myRank = _Server.GetMyRank();

        InitElements();
    }

    void InitElements()
    {
        // Values.Length_RankPage
        elementCount = 0;

        var items = _Server.GetRankLists(Values.Length_RankPage, 0);
        if (items == null)
            return;

        rankCount = items.Count;

        bool isMineInRank = false;

        GameObject tierObj = null;
        foreach (var item in items)
        {
            tierObj = item.rank <= topTierObjs.Length ? topTierObjs[item.rank - 1] : null;

            // 랭킹 안들어가고 닉네임 / 캐릭터 변경한 경우
            if (item.inDate == _Server.Owner_InDate)  
            {
                RankInfo rankInfo = item;
                rankInfo.nickname = _Server.Nickname.Value;
                rankInfo.iconNum = DataManager.Instance.Profile.Value;

                myElement.InitializeWithData(rankInfo, tierObj);
                rankElements[elementCount].InitializeWithData(rankInfo, tierObj);
                isMineInRank = true;

                rankInMyElement = rankElements[elementCount];
            }
            else
            {
                rankElements[elementCount].InitializeWithData(item, tierObj);
            }

            elementCount++;
        }

        for (int i = elementCount; i < Values.Length_RankPage; i++)
        {
            rankElements[i].gameObject.SetActive(false);
        }

        // 내 정보가 없는 경우 = 데이터 기록을 안한경우 / 랭킹 밖
        if(!isMineInRank)
        {
            if (myRank > 0)     // 랭킹 밖
            {   // (string nickname, int iconNum, int rank, int score, string inDate)
                myElement.InitializeWithData(new RankInfo
                (
                    _Server.Nickname.Value,
                    _DataManager.Profile.Value,
                    myRank,
                    _DataManager.Height.Value,
                    ""
                ), null);
            }
            else                    // 기록 없는 경우
            {
                myElement.InitializeWithData(new RankInfo
                (
                    _Server.Nickname.Value,
                    DataManager.Instance.Profile.Value,
                    -1,
                    0,
                    ""
                ), null);
            }
        }
    }

    void UpdateMyDatas()
    {
        string nickname = BackendManager.Instance.Nickname.Value;
        int iconNum = DataManager.Instance.Profile.Value;
        // Debug.Log(iconNum);
        if(rankInMyElement != null)
            rankInMyElement.UpdateData(nickname, iconNum);
        myElement.UpdateData(nickname, iconNum);
    }

    void UpdateElementDatas()
    {
        for (int i = 0; i < rankCount; i++)
        {
            rankElements[i].UpdateData();
        }
    }

    void CreateElements(int start, int end)
    {

    }
}
