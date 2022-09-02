using UnityEngine;
using UniRx;

public class SystemUI : Singleton<SystemUI>
{
    [SerializeField] OneButtonUI oneButtonUI;
    [SerializeField] TwoButtonUI twoButtonUI;
    [SerializeField] NoneTouchUI noneTouchUI;

    [SerializeField] InputButtonUI inputButtonUI;
    [SerializeField] RewardUI rewardUI;


    protected override void AwakeInstance() { }
    protected override void DestroyInstance() { }

    private void Start()
    {
        var keyDown = Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.Escape));

        keyDown
            .Subscribe(_ => QuitMessage())
            .AddTo(this);
    }

    public void QuitMessage()
    {
        OpenTwoButton(Values.Local_Entry_Message, Values.Local_Entry_Quit, Values.Local_Entry_Confirm, Values.Local_Entry_Cancel,
                                () => GameApplication.Instance.Quit(), null);
    }

    public void OpenOneButton(string _titleEntry, string _contentEntry, string _buttonEntry, System.Action _buttonAction = null)
    => oneButtonUI.OpenWithData(_titleEntry, _contentEntry, _buttonEntry, _buttonAction);

    public void OpenOneButton(string _titleEntry, string _contentTable, string _contentEntry, string _buttonEntry, System.Action _buttonAction = null)
    => oneButtonUI.OpenWithData(_titleEntry, _contentTable, _contentEntry, _buttonEntry, _buttonAction);

    public void OpenTwoButton(string _titleEntry, string _contentEntry, string _leftButtonEntry, string _rightButtonEntry,
                                        System.Action _leftButtonAction = null, System.Action _rightButtonAction = null)
    => twoButtonUI.OpenWithData(_titleEntry, _contentEntry, _leftButtonEntry, _rightButtonEntry, _leftButtonAction, _rightButtonAction);

    public void OpenTwoButton(string _titleEntry, string _contentTable, string _contentEntry, string _leftButtonEntry, string _rightButtonEntry,
                                    System.Action _leftButtonAction = null, System.Action _rightButtonAction = null)
=> twoButtonUI.OpenWithData(_titleEntry, _contentTable, _contentEntry, _leftButtonEntry, _rightButtonEntry, _leftButtonAction, _rightButtonAction);

    //public void OpenTwoButton(string _titleText, string _contentsText, string _leftButtonText, string _rightButtonText,
    //                                    System.Action _leftButtonAction = null, System.Action _rightButtonAction = null)
    //=> twoButtonUI.OpenWithData(_titleText, _contentsText, _leftButtonText, _rightButtonText, _leftButtonAction, _rightButtonAction);

    public void OpenNoneTouch(string _contentsText)
    => noneTouchUI.OpenWithData(_contentsText);

    public void OpenNoneTouch(string _table, string _entry)
    => noneTouchUI.OpenWithData(_table, _entry);

    public void OpenInputButton(EInputType type, System.Action<string> _leftButtonAction = null)
    => inputButtonUI.OpenWithData(type, _leftButtonAction);

    public void OpenInputButton(int _charLimit, int _titleNum, int _contentsNum,
                                    System.Action<string> _leftButtonAction = null, System.Action<string> _rightButtonAction = null)
    => inputButtonUI.OpenWithData(_charLimit, _titleNum, _contentsNum, _leftButtonAction, _rightButtonAction);

    // length
    public void OpenReward(System.Collections.Generic.List<FGetItem> getItems, System.Action onReward = null)
    => rewardUI.OpenWithData(getItems, onReward);
}
