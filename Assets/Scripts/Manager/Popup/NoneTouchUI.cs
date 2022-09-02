using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;

public class NoneTouchUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI contentsText;

    public void OpenWithData(string _contentsText)
    {
        contentsText.text = _contentsText;

        gameObject.SetActive(true);
    }

    public void OpenWithData(string table, string key)
    {
        contentsText.text = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);

        gameObject.SetActive(true);
    }

    public void OpenWithData(int _contentsNum)
    {
        // contentsText.text = _contentsNum.Localization();

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
