using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UnityEngine.Localization;

public class ProfileUI : PopupUI
{
    [SerializeField] TextMeshProUGUI nicknameText;
    [SerializeField] TextMeshProUGUI rankText;
    [SerializeField] TextMeshProUGUI heightText;
    [SerializeField] Button modifyButton;
    [SerializeField] Image profileIcon; 

    BackendManager _Server;

    private int rank;
    private bool isUpdate;
    private LocalizedString localizedHeight;


    protected override void Awake()
    {
        base.Awake();

        localizedHeight = new LocalizedString(tableReference: Values.Local_Table_Profile, entryReference: Values.Local_Entry_Height)
        {
            {Values.Local_Name_BestHeight, new UnityEngine.Localization.SmartFormat.PersistentVariables.IntVariable{Value = _DataManager.Height.Value }}
        };
    }

    protected override void Start()
    {
        base.Start();

        modifyButton.onClick.AddListener(OnModify);

        AddObserves();
    }

    void AddObserves()
    {
        // **Language ����

        //_DataManager.Height.SubscribeToText(heightText, value => $"{value}��")
        //    .AddTo(this.gameObject);

        _Server.Nickname.SubscribeToText(nicknameText)
            .AddTo(this.gameObject);

        _DataManager.Profile
            .Subscribe(value => profileIcon.sprite = GameData.Instance.CharacterSprite(value))
            .AddTo(this.gameObject);
    }

    protected override void UpdateData()
    {
        if(_Server == null)
            _Server = BackendManager.Instance;

        // �ΰ��� ���� ���ϸ� �ȹޱ�
        if(!isUpdate)
        {
            rank = _Server.GetMyRank();
            isUpdate = true;
        }

        rankText.text = rank.OrdinalNumber();
        heightText.text = localizedHeight.GetLocalizedString();
    }

    private void OnModify()
    {
        SystemUI.Instance.OpenInputButton(EInputType.ModifyNickname, 
                                                    (value) => _Server.UpdateNickname(value, null));
    }
}
