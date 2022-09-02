using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForumUI : MonoBehaviour
{
    [SerializeField] ScrollRect scroll;

    [SerializeField] GameObject[] engObjs;
    [SerializeField] GameObject[] korObjs;

    GameApplication _GameApplication;

    void Start()
    {
        _GameApplication = GameApplication.Instance;
    }

    public void UpdateData()
    {
        scroll.verticalNormalizedPosition = 1f;

        SetLanguageObjs();
    }

    public void SetLanguageObjs()
    {
        foreach (var obj in engObjs)
        {
            obj.SetActive(false);
        }
        foreach (var obj in korObjs)
        {
            obj.SetActive(true);
        }
    }

    public void OnForumSite(string website)
    {
        // _GameApplication.ShowWebView(website);
    }

    public void OnApplicationSite(string market)
    {
        Application.OpenURL(market);
    }
}
