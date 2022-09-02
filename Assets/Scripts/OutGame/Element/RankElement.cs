using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.AddressableAssets;

public class RankElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rankText;              // �� or ����ǥ��
    [SerializeField] TextMeshProUGUI nicknameText;      
    [SerializeField] TextMeshProUGUI scoreText;             // �� or F
    // [SerializeField] Image rankImage;
    [SerializeField] Image icon;
    [SerializeField] Transform rankImgParent;

    RankInfo data;

    LocalizedString localizedHeight;


    public void InitializeWithData(RankInfo rankInfo, GameObject rankImgObj)
    {
        data = rankInfo;

        nicknameText.text = $"{rankInfo.nickname}";

        localizedHeight = new LocalizedString(tableReference: Values.Local_Table_Profile, entryReference: Values.Local_Entry_Height)
        {
            {Values.Local_Name_BestHeight, new UnityEngine.Localization.SmartFormat.PersistentVariables.IntVariable{Value = rankInfo.score }}
        };
        // Debug.Log($"�� : {localizedHeight.GetLocalizedString()}");

        if (rankImgObj != null)
        {
            // **Addressable �ʿ� : ����� ���� �ȵ�
            Debug.Log(rankImgObj.name);
            //Addressables.InstantiateAsync(rankImgObj.name, rankImgParent).WaitForCompletion();
            Instantiate(rankImgObj, rankImgParent);
            // var handle = Addressables.LoadAssetAsync<GameObject>(rankImgObj).Completed();
        }
        // rankImage.enabled = rankInfo.rank <= Values.Length_TopRank;

        icon.sprite = GameData.Instance.CharacterSprite(rankInfo.iconNum);

        UpdateData();
    }

    public void UpdateData(string nickname, int iconNum)
    {
        nicknameText.text = $"{nickname}";
        icon.sprite = GameData.Instance.CharacterSprite(iconNum);

        rankText.text = data.rank.OrdinalNumber();
        scoreText.text = localizedHeight.GetLocalizedString();
    }

    public void UpdateData()
    {
        rankText.text = data.rank.OrdinalNumber();
        scoreText.text = localizedHeight.GetLocalizedString();
    }
}