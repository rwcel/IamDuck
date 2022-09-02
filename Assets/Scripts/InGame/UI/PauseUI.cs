using UnityEngine;
using UnityEngine.UI;

namespace InGameUIs
{
    public class PauseUI : PopupUI
    {
        [SerializeField] Button retryButton;
        [SerializeField] Button exitButton;

        protected override void Start()
        {
            base.Start();

            closeButton.onClick.AddListener(Continue);
            retryButton.onClick.AddListener(RetryGame);
            exitButton.onClick.AddListener(ExitGame);
        }

        protected override void UpdateData()
        {
            Time.timeScale = 0f;
            AudioManager.Instance.PauseBGM(true);
            InGameManager.Instance.IsPause = true;
        }

        void Continue()
        {
            Time.timeScale = 1f;
            AudioManager.Instance.PauseBGM(false);
            InGameManager.Instance.IsPause = false;
        }

        void ResetData()
        {
            Time.timeScale = 1f;
            AudioManager.Instance.PauseBGM(false);
            PoolingManager.Instance.AllEnqueueObjects();
            _DataManager.ResetUseItems();
        }

        void RetryGame()
        {
            // Values.Local_Table_InGame
            SystemUI.Instance.OpenTwoButton(Values.Local_Entry_Message, Values.Local_Table_InGame, Values.Local_Entry_Retry,
                                                    Values.Local_Entry_Confirm, Values.Local_Entry_Cancel, OnRetry);
        }

        void ExitGame()
        {
            SystemUI.Instance.OpenTwoButton(Values.Local_Entry_Message, Values.Local_Table_InGame, Values.Local_Entry_Exit,
                                                    Values.Local_Entry_Confirm, Values.Local_Entry_Cancel, OnExit);
        }

        void OnRetry()
        {
            ResetData();
            // InGameManager.Instance.SaveData();           // 저장 안하기
            GameSceneManager.Instance.MoveScene(EScene.InGame, ETransition.Vertical);
        }

        void OnExit()
        {
            ResetData();
            GameSceneManager.Instance.MoveScene(EScene.OutGame, ETransition.Vertical);
        }
    }
}