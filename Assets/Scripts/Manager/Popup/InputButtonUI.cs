using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using static Values;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization;

/// <summary>
/// �� Yes, �� No ���� Input
/// </summary>
public class InputButtonUI : MonoBehaviour
{
    // SO?
    [System.Serializable]
    private struct FInputData
    {
        public EInputType type;
        public int charLimit;
        public string table;
        public string titleKey;
        public string contentKey;
        // public string titleText;
        // public string contentText;      // Language ���� �Ѵ� int�� ���� �ʿ�
        public bool isTwoButton;
    }

    [BoxGroup("������")]
    [SerializeField] FInputData[] inputDatas;

    FInputData InputTypeData(EInputType type) => inputDatas[(int)type];         // ������ζ�� �����Ͽ�

    // private Dictionary<EInputType, FInputData> inputTypeDatas = new();

    [BoxGroup("UI")]
    [SerializeField] TMP_InputField inputField;
    [BoxGroup("UI")]
    [SerializeField] TextMeshProUGUI titleText;
    [BoxGroup("UI")]
    [SerializeField] TextMeshProUGUI contentsText;          // PlaceHolder
    [BoxGroup("UI")]
    [SerializeField] Button okButton;
    [BoxGroup("UI")]
    [SerializeField] Button cancelButton;

    System.Action<string> okButtonAction;
    // System.Action<string> cancelButtonAction;

    private Animator anim;
    private AnimEvent animEvent;

    private FInputData curData;

    public static readonly int _Anim_Close = Animator.StringToHash("Close");

    SystemUI _SystemUI;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        animEvent = GetComponent<AnimEvent>();

        _SystemUI = SystemUI.Instance;
    }

    private void Start()
    {
        okButton.onClick.AddListener(() => OnConfirm(okButtonAction));
        cancelButton.onClick.AddListener(OnCancel);
    }


    public void OpenWithData(EInputType type, System.Action<string> _okButtonAction = null)
    {
        curData = InputTypeData(type);

        inputField.text = "";
        inputField.characterLimit = curData.charLimit;

        titleText.text = LocalizationSettings.StringDatabase.GetLocalizedString(curData.table, curData.titleKey);
        contentsText.text = LocalizationSettings.StringDatabase.GetLocalizedString(curData.table, curData.contentKey);

        okButtonAction = _okButtonAction;

        // cancelButtonAction = null;

        cancelButton.gameObject.SetActive(curData.isTwoButton);
            

        gameObject.SetActive(true);
    }

    public void OpenWithData(int _charLimit, int _titleNum, int _contentNum,
                                System.Action<string> _okButtonAction = null, System.Action<string> _cancelButtonAction = null)
    {
        inputField.text = "";
        inputField.characterLimit = _charLimit;

        //titleText.text = _titleNum.Localization();
        //contentsText.text = string.Format(_contentsNum.Localization(), _charLimit);

        //leftButton.onClick.AddListener(() => OnConfirm(_leftButtonAction));
        //rightButton.onClick.AddListener(() => OnConfirm(_rightButtonAction));

        gameObject.SetActive(true);
    }

    void OnConfirm(System.Action<string> buttonAction)
    {
        //AudioManager.Instance.PlaySFX(ESFX.Touch);

        bool flag = false;
        switch (curData.type)
        {
            case EInputType.CreateNickname:
                flag = DuplicateNickname();
                break;
            case EInputType.ModifyNickname:
                flag = DuplicateNickname();
                break;
            case EInputType.Coupon:
                flag = CheckCoupon();
                break;
        }

        if (!flag)
        {
            inputField.text = "";
            return;
        }

        buttonAction?.Invoke(inputField.text);

        anim.SetTrigger(_Anim_Close);
    }

    void OnCancel()
    {
        anim.SetTrigger(_Anim_Close);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    private static readonly int Min_Byte = 4;
    private static readonly int Max_Byte = 16;

    public bool DuplicateNickname()
    {
        string inputText = inputField.text;
        var data = System.Text.Encoding.Unicode.GetBytes(inputText);
        //var data1 = inputText.ToCharArray();
        //var data2 = System.Text.Encoding.Default.GetBytes(inputText);
        //var data3 = System.Text.Encoding.Unicode.GetBytes(inputText);
        //var data4 = System.Text.Encoding.UTF8.GetBytes(inputText);
        //var data5 = System.Text.Encoding.ASCII.GetBytes(inputText);
        //Debug.Log($"{data1.Length} {data2.Length} {data3.Length} {data4.Length} {data5.Length}");

        if (data.Length > Max_Byte || data.Length < Min_Byte)
        {
            _SystemUI.OpenNoneTouch(Local_Table_Nickname, Local_Entry_Byte);
            //Debug.LogWarning("byte ���� ����");
            return false;
        }

        if (Regex.IsMatch(inputText, @"[^a-zA-Z0-9��-�R]"))
        // if (!Regex.IsMatch(inputText, @"[0-9a-zA-Z��-�R]"))
        {
            // �ѱ�, ����, ����
            _SystemUI.OpenNoneTouch(Local_Table_Nickname, Local_Entry_IsMatch);
            //Debug.LogWarning("�ѱ�, ����, ���ڸ� ����");
            return false;
        }

        // BadWord

        if (!BackendManager.Instance.CheckDuplicateNickname(inputText))
        {
            // �ߺ� �г���
            _SystemUI.OpenNoneTouch(Local_Table_Nickname, Local_Entry_Duplicate);
            //Debug.LogWarning("�ߺ� �г���");
            return false;
        }

        return true;
    }

    /// <summary>
    /// �ڸ��� �˻� ���� ���� : �´��� Ȯ�θ� �ϸ� �Ǳ⶧����
    /// </summary>
    /// <returns></returns>
    public bool CheckCoupon()
    {
        return true;
        //if (BackEndServerManager.Instance.IsValidCoupon(_text))
        //{
        //    _GamePopup.OpenPopup(EGamePopup.Reward, null, () => _GamePopup.AllClosePopup(null));
        //}
        //else
        //{
        //    SystemUI.Instance.OpenNoneTouch(55);
        //}
    }
}
