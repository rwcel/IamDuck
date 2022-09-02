using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewsUI : MonoBehaviour
{
    [SerializeField] ScrollRect scroll;
    [SerializeField] Transform listParent;
    [SerializeField] GameObject elementPrefab;

    // 한번만 실행하기
    void Awake()
    {
        BackendManager server = BackendManager.Instance;

        var news = server.GetNews();
        foreach (var item in news)
        {
            var element = Instantiate(elementPrefab, listParent).GetComponent<NewsElement>();
            element.InitializeWithData(item.Key, item.Value);
        }
    }

    public void UpdateData()
    {
        scroll.verticalNormalizedPosition = 1f;
    }
}
