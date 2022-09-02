using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static EOutGamePopup;
using static EInGamePopup;

public class Popup
{
    public PopupUI ui;
    public System.Action closeAction;

    public Popup(PopupUI ui, Action closeAction)
    {
        this.ui = ui;
        this.closeAction = closeAction;
    }
}

public class GamePopup : Singleton<GamePopup>
{
    protected List<PopupUI> popups;          // ��ųʸ�?

    public Stack<Popup> popupStack = new Stack<Popup>();

    private bool isDestroyed;

    protected override void AwakeInstance()
    {
        popups = new List<PopupUI>();
        foreach (Transform child in transform)
        {
            popups.Add(child.GetComponent<PopupUI>());
            child.gameObject.SetActive(false);
        }
    }

    protected override void DestroyInstance() 
    {
        isDestroyed = true;
        popups.Clear();
    }

    public void OpenPopup(int typeNum, bool isTouch = true, Action openAction = null, Action closeAction = null)
    {
        if (isDestroyed)
        {
            Debug.LogWarning("�ı���");
            return;
        }
        if (popups.Count <= typeNum)
        {
            Debug.LogWarning("�˾� ������ ��� ��û");
            return;
        }
        if (isTouch)
        {
            AudioManager.Instance.PlaySFX(ESfx.Touch);
        }


        popupStack.Push(new Popup(popups[typeNum], closeAction));
        popups[typeNum].gameObject.SetActive(true);

        openAction?.Invoke();           // ���� ���⼭ �ʿ��Ѱ�?
    }

    public void ClosePopupNotAction()
    {
        if (isDestroyed)
        {
            Debug.LogWarning("�ı���");
            return;
        }
        if (popups.Count <= 0)
        {
            Debug.LogWarning("�˾� ������ ��� ��û");
            return;
        }

        AudioManager.Instance.PlaySFX(ESfx.Touch);

        var offPopup = popupStack.Pop();
        offPopup.ui.PlayCloseAnim(null);
    }

    /// <summary>
    /// BG Ŭ�� ��, �������ϱ�
    /// </summary>
    public void ClosePopup()
    {
        if (isDestroyed)
        {
            Debug.LogWarning("�ı���");
            return;
        }
        if (popups.Count <= 0)
        {
            Debug.LogWarning("�˾� ������ ��� ��û");
            return;
        }

        AudioManager.Instance.PlaySFX(ESfx.Touch);

        var offPopup = popupStack.Pop();
        offPopup.ui.PlayCloseAnim(offPopup.closeAction);
        //offPopup.ui.Close();
    }

    public void AllClosePopup() //(Action closeAction)
    {
        if (isDestroyed)
        {
            Debug.LogWarning("�ı���");
            return;
        }
        if (popups.Count <= 0)
        {
            Debug.LogWarning("�˾� ������ ��� ��û");
            return;
        }

        while (popupStack.Count > 0)
        {
            if (isDestroyed)
                break;

            ClosePopup();
        }

        // closeAction?.Invoke();
    }


    #region �Ű����� ���� ȣ��� �Լ�

    // OutGame
    public void OpenMission() => OpenPopup((int)Mission);
    public void OpenMail() => OpenPopup((int)Mail);
    public void OpenProfile() => OpenPopup((int)Profile);
    public void OpenAttendance(bool isTouch) => OpenPopup((int)Attendance, isTouch);
    public void OpenRank() => OpenPopup((int)Rank);
    public void OpenSetting() => OpenPopup((int)Setting);
    public void OpenNotice(bool isTouch) => OpenPopup((int)Notice, isTouch);
    public void OpenLinkAccount() => OpenPopup((int)LinkAccount);
    public void OpenShop() => OpenPopup((int)Shop);
    public void OpenUnlock() => OpenPopup((int)Unlock);

    // InGame
    public void OpenPause() => OpenPopup((int)Pause);
    public void OpenContinue() => OpenPopup((int)Continue);
    public void OpenGameOver() => OpenPopup((int)GameOver);

    #endregion
}
