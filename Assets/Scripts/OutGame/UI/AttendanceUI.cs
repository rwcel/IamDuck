using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class AttendanceUI : PopupUI
{
    [SerializeField] Transform rewardListTr;
    [SerializeField] OnOffButton recvButton;

    AttendanceElement todayElement;

    private AttendanceElement[] attendanceElements;
    private int today;

    string localizedRecv;
    string localizedClose;


    protected override void Awake()
    {
        base.Awake();

        attendanceElements = new AttendanceElement[rewardListTr.childCount];
        for (int i = 0, length = attendanceElements.Length; i < length; i++)
        {
            attendanceElements[i] = rewardListTr.GetChild(i).GetComponent<AttendanceElement>();
        }

        localizedRecv = LocalizationSettings.StringDatabase.GetLocalizedString(Values.Local_Table_Common, Values.Local_Entry_Recv);
        localizedClose = LocalizationSettings.StringDatabase.GetLocalizedString(Values.Local_Table_Common, Values.Local_Entry_Close);
    }

    protected override void Start()
    {
        base.Start();

        int count = _DataManager.TodayAttendance.Value;
        today = count % attendanceElements.Length;

        todayElement = attendanceElements[today];

        for (int i = 0, length = attendanceElements.Length; i < length; i++)
        {
            attendanceElements[i].InitializeWithData(_DataManager.DailyChecks[i], today + 1, count);
        }

        todayElement.AnimEvent.SetAnimEvent(_GamePopup.ClosePopup);
    }

    /// <summary>
    /// 일일 1번이기때문에 UpdateData에 넣어도 문제 없음
    /// </summary>
    protected override void UpdateData()
    {
        bool canRecv = _DataManager.DailyLogin();
        recvButton.ClickButton(RecvToday, _GamePopup.ClosePopup);
        recvButton.SetButton(canRecv);
        recvButton.SetText(canRecv ? localizedRecv : localizedClose);
    }

    private void RecvToday()
    {
        AudioManager.Instance.PlaySFX(ESfx.Attendance);

        todayElement.PlayReward();
        // dayRewards[today].InitializeWithData(dataManager.DailyChecks[today], false);

        DataManager.Instance.RecvDailyLogin();

        // Close
        // GamePopup.Instance.ClosePopup();
    }
}
