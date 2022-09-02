using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MPUIKIT;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using Cinemachine;

public class OutGameUI : MonoBehaviour
{
    private enum ELobbyState
    {
        Home,
        Manage,
        Character,
    }

    [SerializeField] CinemachineVirtualCamera vcam;

    [BoxGroup("데이터")]
    [SerializeField] TextMeshProUGUI coinText;
    [BoxGroup("데이터")]
    [SerializeField] TextMeshProUGUI diaText;

    [BoxGroup("프로필")]
    [SerializeField] Transform modelParent;
    [BoxGroup("프로필")]
    [SerializeField] TextMeshProUGUI nicknameText;
    [BoxGroup("프로필")]
    [SerializeField] Image profileIcon;
    //[SerializeField] TextMeshProUGUI heightText;

    [BoxGroup("공통")]
    [SerializeField] GameObject adObj;
    [BoxGroup("공통")]
    [SerializeField] Button homeButton;
    [BoxGroup("공통")]
    [SerializeField] Button mailButton;
    [BoxGroup("공통")]
    [SerializeField] Button settingButton;

    [BoxGroup("스타트")]
    [SerializeField] Button startButton;

    [BoxGroup("로비 버튼")]
    [SerializeField] Button upgradeButton;
    [BoxGroup("로비 버튼")]
    [SerializeField] Button characterButton;
    [BoxGroup("로비 UI")]
    [SerializeField] GameObject homeUI;
    [BoxGroup("로비 UI")]
    [SerializeField] ManageUI manageUI;
    [BoxGroup("로비 UI")]
    [SerializeField] CharacterUI characterUI;
    [BoxGroup("로비 UI")]
    [SerializeField] GameObject profileUI;          // 캐릭터 안에 들어가있는 homeUI

    [BoxGroup("알림")]
    [SerializeField] GameObject missionNotify;          // 받을 수 있는게 1개 이상이면?
    [BoxGroup("알림")]
    [SerializeField] GameObject manageNotify;         // 업그레이드 1개 이상?
    [BoxGroup("알림")]
    [SerializeField] GameObject characterNotify;
    [BoxGroup("알림")]
    [SerializeField] GameObject mailNotify;

    [BoxGroup("오버레이 버튼")]
    [SerializeField] Button shopButton;
    [BoxGroup("오버레이 버튼")]
    [SerializeField] Button rankButton;
    [BoxGroup("오버레이 버튼")]
    [SerializeField] Button missionButton;
    [BoxGroup("오버레이 버튼")]
    [SerializeField] Button profileButton;
    [BoxGroup("오버레이 버튼")]
    [SerializeField] Button attendanceButton;

    // [SerializeField] Toggle boostToggle;
    ELobbyState loobyState;

    GamePopup _GamePopup;
    DataManager _DataManager;


    private void Start()
    {
        _GamePopup = GamePopup.Instance;
        _DataManager = DataManager.Instance;

        characterUI.OnAwake(_DataManager.Profile.Value);

        AddListeners();

        SubscribeObserves();

        ChangeLobbyState(ELobbyState.Home, false);

        LoginCheck();

        if(!_DataManager.IsLogin)
        {
            _GamePopup.OpenNotice(false);
        }

        // Intro에서 받으면 데이터 인계 안됨
        if(OnestoreManager.Instance.GetItems.Count > 0)
        {
            SystemUI.Instance.OpenReward(OnestoreManager.Instance.GetItems, OnestoreManager.Instance.ClearItems);
        }
    }

    void SubscribeObserves()
    {
        _DataManager.Dia.SubscribeToText(diaText)
            .AddTo(this.gameObject);

        _DataManager.Coin.SubscribeToText(coinText)
            .AddTo(this.gameObject); ;

        _DataManager.IsMissionNotify.Subscribe(missionNotify.SetActive)
            .AddTo(this.gameObject);

        _DataManager.IsManageNotify.Subscribe(manageNotify.SetActive)
            .AddTo(this.gameObject);            // **초기체크?

        characterUI.Notify.Subscribe(value => characterNotify.SetActive(value > 0))
            .AddTo(this.gameObject);


        BackendManager server = BackendManager.Instance;

        mailNotify.SetActive(server.MailList.Count > 0);
        server.MailCount
            .Subscribe(value => 
            {
                Debug.Log($"mailCount : {value}");
                mailNotify.SetActive(value > 0);
                })
            .AddTo(gameObject);


        server.Nickname.SubscribeToText(nicknameText)
            .AddTo(this.gameObject);

        _DataManager.Profile
            .Subscribe(value => profileIcon.sprite = GameData.Instance.CharacterSprite(value))
            .AddTo(this.gameObject);

        server.RemainAdRemove
            .Subscribe(value => adObj.SetActive(value < System.DateTime.Now))
            .AddTo(this.gameObject);
    }

    void AddListeners()
    {
        startButton.onClick.AddListener(StartGame);

        upgradeButton.onClick.AddListener(() => ChangeLobbyState(ELobbyState.Manage));
        characterButton.onClick.AddListener(() => ChangeLobbyState(ELobbyState.Character));

        shopButton.onClick.AddListener(_GamePopup.OpenShop);
        rankButton.onClick.AddListener(_GamePopup.OpenRank);
        missionButton.onClick.AddListener(_GamePopup.OpenMission);
        mailButton.onClick.AddListener(_GamePopup.OpenMail);
        settingButton.onClick.AddListener(_GamePopup.OpenSetting);
        profileButton.onClick.AddListener(_GamePopup.OpenProfile);
        attendanceButton.onClick.AddListener(() => _GamePopup.OpenAttendance(true));

        homeButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySFX(ESfx.Touch);
            ChangeLobbyState(ELobbyState.Home);
        });

        characterUI.InitializeElements(modelParent);
        characterUI.OnEquip += (value) =>
        {
            ChangeLobbyState(ELobbyState.Home);
            // 서버 전송
            _DataManager.UpdateProfile(value);
        };
    }

    void LoginCheck()
    {
        if (_DataManager.DailyLogin())
        {
            _GamePopup.OpenAttendance(false);
        }
    }

    void StartGame()
    {
        AudioManager.Instance.PlaySFX(ESfx.GameStart);
        GameSceneManager.Instance.MoveScene(EScene.InGame, ETransition.Vertical);

        // ConnectManager.Instance.SelectBoost(boostToggle.isOn);
    }

    private void ChangeLobbyState(ELobbyState state, bool isPlaySFX = true)
    {
        bool isHome = state == ELobbyState.Home;

        homeButton.gameObject.SetActive(!isHome);
        mailButton.gameObject.SetActive(isHome);
        settingButton.gameObject.SetActive(isHome);

        homeUI.SetActive(state == ELobbyState.Home);
        profileUI.SetActive(state == ELobbyState.Home);
        manageUI.gameObject.SetActive(state == ELobbyState.Manage);
        characterUI.gameObject.SetActive(state == ELobbyState.Character);

        switch (state)
        {
            case ELobbyState.Home:
                vcam.transform.DOLocalMoveY(0.8f, 0.2f);
                break;
            case ELobbyState.Manage:
                vcam.transform.DOLocalMoveY(-1.35f, 0.2f);
                break;
            case ELobbyState.Character:
                vcam.transform.DOLocalMoveY(-2f, 0.2f);
                break;
        }

        //if(isPlaySFX)
        //{
        //    AudioManager.Instance.PlaySFX(ESfx.Touch);
        //}
        // 애니메이션 재생?
    }
}
