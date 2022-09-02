using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;

public class InGameUI : MonoBehaviour
{
    [SerializeField] GameObject adObj;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TextMeshProUGUI heightText;
    [SerializeField] TextMeshProUGUI heightUnitText;
    [SerializeField] TextMeshProUGUI bestHeightText;
    [SerializeField] Button pauseButton;
    //[SerializeField] Slider timeSlider;
    //[SerializeField] TextMeshProUGUI powerText;
    [SerializeField] GameObject[] useItemIcons;

    InGameManager _GameManager;

    [SerializeField] Color bestColor;

    private bool isRecord;
    private int tmpCoin;
    Coroutine countingCoroutine;

    private static readonly int _Max_Counting = 25;


    private void Start()
    {
        _GameManager = InGameManager.Instance;
        isRecord = false;
        tmpCoin = 0;

        adObj.SetActive(!BackendManager.Instance.IsAdRemove);

        pauseButton.onClick.AddListener(OpenPause);

        AddSubScribes();

        ShowBuffIcons();
    }

    void AddSubScribes()
    {
        //_GameManager.Coin.SubscribeToText(coinText)
        //    .AddTo(this.gameObject);
        _GameManager.Coin
            .Subscribe(value => CoinCounting(value))
            .AddTo(this.gameObject);

        //var localizedHeight = new LocalizedString(tableReference: Values.Local_Table_InGame, entryReference: Values.Local_Entry_Height);
        //{
        //    {Values.Local_Name_Height, new UnityEngine.Localization.SmartFormat.PersistentVariables.IntVariable{Value = value }}
        //};

        _GameManager.Height.SubscribeToText(heightText)
            .AddTo(this.gameObject);

        _GameManager.RecordHeight.Subscribe(value =>
        {
            if (!isRecord
            && _GameManager.Height.Value >= value)
            //heightText.text != _Key_Zero)
            {
                UpdateRecord();
                Debug.Log($"New Record : {value}");
            }
            bestHeightText.text = value.ToString();
        }).AddTo(this.gameObject);
    }

    void ShowBuffIcons()
    {
        for (int i = 0, length = useItemIcons.Length; i < length; i++)
        {
            if(useItemIcons[i] != null)
            {
                // Debug.Log($"{i} - {DataManager.Instance.SelectedUseItems[i]}");
                useItemIcons[i].SetActive(DataManager.Instance.SelectedUseItems[i]);
            }
        }
    }

    void UpdateRecord()
    {
        isRecord = true;
        heightText.color = bestColor;
        heightUnitText.color = bestColor;
    }


    private void CoinCounting(int endValue)
    {
        //if (endValue != tmpCoin)
        if(countingCoroutine != null)
            StopCoroutine(countingCoroutine);

        countingCoroutine = StartCoroutine(CoCoinCounting(endValue));
    }

    IEnumerator CoCoinCounting(int endValue)
    {
        int perValue = (endValue - tmpCoin) / Mathf.Clamp(endValue - tmpCoin, 1, _Max_Counting);

        while (tmpCoin < endValue)
        {
            yield return Values.Delay002;
            tmpCoin += perValue;
            coinText.text = tmpCoin.ToString();
        }

        tmpCoin = endValue;
        coinText.text = tmpCoin.ToString();

        countingCoroutine = null;
    }

    private void OpenPause()
    {
        if (!_GameManager.CanTouchUI)
            return;

        GamePopup.Instance.OpenPause();
    }
}
