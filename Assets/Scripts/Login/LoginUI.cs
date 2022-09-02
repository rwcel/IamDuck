using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MPUIKIT;
using TMPro;
using Sirenix.OdinInspector;

public class LoginUI : MonoBehaviour
{
    [BoxGroup("하위 UI")]
    [SerializeField] PrivacyUI privacyUI;
    [BoxGroup("하위 UI")]
    [SerializeField] GameObject accountUI;

    [SerializeField] TextMeshProUGUI versionText;
    [SerializeField] Button startButton;

    [BoxGroup("계정")]
    [SerializeField] Button googleButton;
    // [SerializeField] Button facebookButton;
    [BoxGroup("계정")]
    [SerializeField] Button guestButton;

    // private bool isDuplicateCheck = false;

    private Animator anim;

    BackendManager _Server;

    private static readonly int _Anim_Start = Animator.StringToHash("Start");


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        _Server = BackendManager.Instance;

        _Server.AllLoadAction = EnterGame;

        versionText.text = $"v{Application.version}";

        startButton.onClick.AddListener(() =>
        {
            _Server.LoginWithTheBackendToken(AllCloseUI, ShowAccountUI);
            startButton.enabled = false;            // 연속 클릭 못하게
        });

        googleButton.onClick.AddListener(() => _Server.GoogleLogin(AllCloseUI, ShowPrivacyUI));
        //facebookButton.onClick.AddListener(backendManager.FacebookLogin);
        guestButton.onClick.AddListener(GuestCheck);
    }

    public void AllCloseUI()
    {
        privacyUI.gameObject.SetActive(false);
        accountUI.SetActive(false);
    }

    public void ShowAccountUI()
    {
        accountUI.SetActive(true);
    }

    public void ShowPrivacyUI()
    {
        AllCloseUI();
        privacyUI.OpenWithData(ShowNicknameUI);
    }

    public void ShowNicknameUI()
    {
        AllCloseUI();

        SystemUI.Instance.OpenInputButton(EInputType.CreateNickname, (value) =>_Server.CreateNickname(value));

        // nicknameUI.SetActive(true);
    }

    private void GuestCheck()
    {
        SystemUI.Instance.OpenTwoButton(
            Values.Local_Entry_Message, Values.Local_Entry_GuestLogin, Values.Local_Entry_Confirm, Values.Local_Entry_Cancel,
            () => _Server.GuestLogin(AllCloseUI, ShowPrivacyUI));
        // SystemPopupUI.Instance.OpenTwoButton(15, 116, 0, 1, BackEndServerManager.Instance.GuestLogin, null);
    }

    void EnterGame()
    {
        anim.SetTrigger(_Anim_Start);
    }

    // animEvent
    public void AnimEventEnterGame()
    {
        GameSceneManager.Instance.MoveScene(EScene.OutGame, ETransition.Vertical);
    }
}
