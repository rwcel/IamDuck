using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class PrivacyUI : MonoBehaviour
{
    [BoxGroup("�̿���")]
    [SerializeField] Toggle termToggle;
    [BoxGroup("��������")]
    [SerializeField] Toggle policyToggle;
    [BoxGroup("Ǫ�þ˸�")]
    [SerializeField] Toggle pushToggle;

    [BoxGroup("�̿���")]
    [SerializeField] Button termButton;
    [BoxGroup("��������")]
    [SerializeField] Button policyButton;

    [BoxGroup("��ư")]
    [SerializeField] Button allAgreeButton;
    [BoxGroup("��ư")]
    [SerializeField] OnOffButton agreeButton;

    System.Action agreeAction;

    private void Start()
    {
        agreeButton.ClickButton(OnAgree, () => SystemUI.Instance.OpenNoneTouch(Values.Local_Table_Intro, Values.Local_Entry_WarningEssential));
        allAgreeButton.onClick.AddListener(OnAllAgree);

        termButton.onClick.AddListener(OnWebTerm);
        policyButton.onClick.AddListener(OnWebPolicy);

        termToggle.onValueChanged.AddListener(CheckAgreeButton);
        policyToggle.onValueChanged.AddListener(CheckAgreeButton);
        pushToggle.onValueChanged.AddListener(CheckPush);

        // ���� ��
        agreeButton.SetButton(false);
    }

    public void OpenWithData(System.Action _agreeAction)
    {
        agreeAction = _agreeAction;

        gameObject.SetActive(true);
    }

    void OnWebTerm()
    {
        // **GPM SDK ��ġ �ʿ�
        // GameApplication.Instance.ShowWebView("AbleX", "http://ablegames.co.kr/terms-of-service");
        Application.OpenURL("http://ablegames.co.kr/terms-of-service");
    }

    void OnWebPolicy()
    {
        // GameApplication.Instance.ShowWebView("AbleX", "http://ablegames.co.kr/privacy-policy");
        Application.OpenURL("http://ablegames.co.kr/privacy-policy");
    }

    void CheckAgreeButton(bool value)
    {
        agreeButton.SetButton(termToggle.isOn && policyToggle.isOn);
    }

    void CheckPush(bool value)
    {
        BackendManager.Instance.SetPushNotification(value);
    }

    void OnAllAgree()
    {
        termToggle.isOn = true;
        policyToggle.isOn = true;

        CheckPush(true);

        OnAgree();
    }

    void OnAgree()
    {
        agreeAction?.Invoke();
    }
}
