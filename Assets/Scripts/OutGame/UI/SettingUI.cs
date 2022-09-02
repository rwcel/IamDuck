using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Localization.Settings;

public class SettingUI : PopupUI
{
    [BoxGroup("���")]
    [SerializeField] Sprite offSprite;
    [BoxGroup("���")]
    [SerializeField] Sprite onSprite;

    [BoxGroup("����")]
    [SerializeField] Button bgmButton;
    [BoxGroup("����")]
    [SerializeField] Button sfxButton;
    [BoxGroup("����")]
    [SerializeField] Button notiButton;
    [BoxGroup("����")]
    [SerializeField] Button languageButton;

    [BoxGroup("���̵�")]
    [SerializeField] Button cafeButton;
    [BoxGroup("���̵�")]
    [SerializeField] Button policyButton;
    [BoxGroup("���̵�")]
    [SerializeField] Button guideButton;
    [BoxGroup("���̵�")]
    [SerializeField] Button noticeButton;
    [BoxGroup("���̵�")]
    [SerializeField] Button couponButton;

    [BoxGroup("����")]
    [SerializeField] TextMeshProUGUI uuidText;
    [BoxGroup("����")]
    [SerializeField] Button uuidCopyButton;
    [BoxGroup("����")]
    [SerializeField] Button linkAccountButton;
    [BoxGroup("����")]
    [SerializeField] TextMeshProUGUI versionText;

    private bool isNoti;

    private int languageLength;
    private int curLanguageNum;

    AudioManager _AudioManager;

    protected override void Awake()
    {
        base.Awake();

        _AudioManager = AudioManager.Instance;

        isNoti = BackendManager.Instance.IsNoti;

        languageLength = LocalizationSettings.AvailableLocales.Locales.Count;
        for (int i = 0, length = LocalizationSettings.AvailableLocales.Locales.Count; i < length; i++)
        {
            if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[i])
            {
                curLanguageNum = i;
                break;
            }
        }
    }

    protected override void Start()
    {
        base.Start();

        SetTexts();
        AddListeners();
    }

    void SetTexts()
    {
        uuidText.text = $"{BackendManager.Instance.UUID}";
        versionText.text = $"Ver.{Application.version}";

        bgmButton.image.sprite = _AudioManager.OnOffBGM ? onSprite : offSprite;
        sfxButton.image.sprite = _AudioManager.OnOffSFX ? onSprite : offSprite;
        notiButton.image.sprite = isNoti ? onSprite : offSprite;
    }

    private void AddListeners()
    {
        bgmButton.onClick.AddListener(SwitchBGM);
        sfxButton.onClick.AddListener(SwitchSFX);
        notiButton.onClick.AddListener(SwitchNoti);
        languageButton.onClick.AddListener(SwitchLanguage);

        cafeButton.onClick.AddListener(() => Application.OpenURL("http://m.cafe.naver.com/ca-fe/web/cafes/30546852/menus/1#"));
        policyButton.onClick.AddListener(() => Application.OpenURL("http://ablegames.co.kr/terms-of-service"));
        guideButton.onClick.AddListener(() => Application.OpenURL("https://cafe.naver.com/ablegames/85"));
        noticeButton.onClick.AddListener(() => GamePopup.Instance.OpenNotice(true));
        couponButton.onClick.AddListener(() => SystemUI.Instance.OpenInputButton(EInputType.Coupon, CheckCoupon));

        uuidCopyButton.onClick.AddListener(OnCopy);
        linkAccountButton.onClick.AddListener(OnAccount);
    }

    protected override void UpdateData()
    {
    }

    private void SwitchBGM()
    {
        bgmButton.image.sprite = _AudioManager.SwitchBGM() ? onSprite : offSprite;
    }
    private void SwitchSFX()
    {
        sfxButton.image.sprite = _AudioManager.SwitchSFX() ? onSprite : offSprite;
    }
    private void SwitchNoti()
    {
        isNoti = !isNoti;
        notiButton.image.sprite = isNoti ? onSprite : offSprite;

        BackendManager.Instance.SetPushNotification(isNoti);

        // Nonetouch
    }

    private void SwitchLanguage()
    {
        if (++curLanguageNum >= languageLength)
            curLanguageNum = 0;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[curLanguageNum];
        Debug.Log($"{LocalizationSettings.SelectedLocale.LocaleName} / {LocalizationSettings.SelectedLocale.Identifier}");
    }

    private void CheckCoupon(string text)
    {
        var data = BackendManager.Instance.IsValidCoupon(text);
        if (data != null)
        {
            SystemUI.Instance.OpenReward(data);
        }
        else
        {
            SystemUI.Instance.OpenNoneTouch(Values.Local_Table_Setting, Values.Local_Entry_NullCoupon);
        }
    }

    private void OnCopy()
    {
        SystemUI.Instance.OpenNoneTouch(Values.Local_Table_Setting, Values.Local_Entry_Copied);
        GUIUtility.systemCopyBuffer = uuidText.text;
    }
    private void OnAccount()
    {
        GamePopup.Instance.OpenLinkAccount();
    }
}
