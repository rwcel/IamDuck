using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class NewsElement : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI contentText;

    private LayoutElement layoutElement;
    private Toggle toggle;
    [SerializeField] RectTransform contentRectTr;

    private static readonly float offY = 120f;
    private static readonly float bottomY = 20f;            // �ؽ�Ʈ �Ʒ� ������ 
    private float onY = 0f;

    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
        toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener(Accordion);
    }

    public void InitializeWithData(string title, string content)
    {
        titleText.text = title;
        contentText.text = content;

        toggle.isOn = false;
        layoutElement.preferredHeight = offY;

    }

    /// <summary>
    /// Initialize���� �ڷ�ƾ�� ����ȵ�
    /// </summary>
    private void OnEnable()
    {
        if(onY == 0f)
        {
            StartCoroutine(nameof(CoDelayUI));
        }
    }

    IEnumerator CoDelayUI()
    {
        yield return Values.DelayFrame;
        onY = offY + bottomY + contentRectTr.rect.height;
    }


    void Accordion(bool isOn)
    {
        layoutElement.preferredHeight = isOn ? onY : offY;
    }
}
