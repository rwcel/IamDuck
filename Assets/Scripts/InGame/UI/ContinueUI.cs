using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueUI : PopupUI
{
    [SerializeField] Button reviveButton;

    protected override void Start()
    {
        base.Start();

        closeButton.onClick.AddListener(_GamePopup.OpenGameOver);
        reviveButton.onClick.AddListener(Revive);
    }

    protected override void UpdateData()
    {
        AudioManager.Instance.PauseBGM(true);
        Time.timeScale = 0f;
    }

    void Revive()
    {
        DataManager.Instance.UseItemType(EUseItemType.Continue);

        InGameManager.Instance.ChangePlayerState(EPlayerState.Revive);

        // 부활시에는 바로 실행되게
        closeAnim = false;
        AudioManager.Instance.PauseBGM(false);
        GamePopup.Instance.ClosePopup();
        Time.timeScale = 1f;
    }
}
