using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Shop;

public class ShopUI : PopupUI
{
    private enum EState
    {
        Goods,
        Package,
    }

    // *����� ����Ϸ� ������ ��ũ�� �� onvaluechange�� ��ġ�� �����Ǳ� ������ ������
    [BoxGroup("��ư")]
    [SerializeField] OnOffButton goodsButton;
    [BoxGroup("��ư")]
    [SerializeField] OnOffButton packageButton;

    [BoxGroup("��ũ��")]
    [SerializeField] RectTransform packageTr;

    [SerializeField] Transform elementListTr;
    private ShopElement[] shopElements;

    private EState state;
    private float packageY;


    protected override void Awake()
    {
        base.Awake();

        //shopElements = new ShopElement[elementListTr.childCount];
        //for (int i = 0, length = shopElements.Length; i < length; i++)
        //{
        //    shopElements[i] = elementListTr.GetChild(i).GetComponent<ShopElement>();
        //}

        // y���� -�̱⶧���� 1�����ֱ�
        packageY = 1 + Mathf.RoundToInt(packageTr.localPosition.y / scrollRect.content.rect.height * 100) * 0.01f;
        // Debug.Log(packageY);
    }

    protected override void Start()
    {
        base.Start();

        AddListeners();
    }

    private void AddListeners()
    {
        goodsButton.ClickButton(() => SwitchToggle(EState.Goods), () => SwitchToggle(EState.Goods));
        packageButton.ClickButton(() => SwitchToggle(EState.Package), () => SwitchToggle(EState.Package));

        scrollRect.onValueChanged.AddListener(CheckToggle);
    }

    protected override void UpdateData()
    {
        // elements Update
        state = EState.Goods;
        goodsButton.SetButton(true);
        packageButton.SetButton(false);
    }

    private void SwitchToggle(EState state)
    {
        // Debug.Log(state);
        // �ش� ��ġ�� ������
        MoveScroll(state);
    }

    void MoveScroll(EState state)
    {
        scrollRect.verticalScrollbar.value = (state == EState.Goods)
                                                    ? 1f
                                                    : packageY - 0.04f;
    }

    void CheckToggle(Vector2 pos)
    {
        // �Ҽ� ù°�ڸ� �ݿø�
        //if(state == EState.Goods 
        //    && pos)
        // Debug.Log($"{pos.y} >= {packageY}");

        // 0.4f
        goodsButton.SetButton(pos.y > packageY);
        packageButton.SetButton(pos.y <= packageY);

    }

}
