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

    // *토글을 사용하려 했으나 스크롤 시 onvaluechange로 위치가 고정되기 때문에 비적합
    [BoxGroup("버튼")]
    [SerializeField] OnOffButton goodsButton;
    [BoxGroup("버튼")]
    [SerializeField] OnOffButton packageButton;

    [BoxGroup("스크롤")]
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

        // y값이 -이기때문에 1더해주기
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
        // 해당 위치로 보내기
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
        // 소수 첫째자리 반올림
        //if(state == EState.Goods 
        //    && pos)
        // Debug.Log($"{pos.y} >= {packageY}");

        // 0.4f
        goodsButton.SetButton(pos.y > packageY);
        packageButton.SetButton(pos.y <= packageY);

    }

}
