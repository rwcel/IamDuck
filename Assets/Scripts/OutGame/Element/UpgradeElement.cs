using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class UpgradeElement : MonoBehaviour
{
    [SerializeField] UpgradeData data;
    public UpgradeData Data => data;

    [SerializeField] Toggle toggle;
    public Toggle Toggle => toggle;

    [SerializeField] Image iconImage;
    [SerializeField] GameObject notify;
    [SerializeField] Slider levelBar;
    public float SliderValue => levelBar.value;

    // public System.Action<int> OnBuy;

    private int cost;
    public int Cost => cost;

    private int level;
    public int Level => level;

    public float UpgradeValue => data.GetUpgradeValue(level);
    public string UpgradeText => data.GetUpgradeText(level);

    DataManager _DataManager;

    private void Start()
    {
        _DataManager.Coin
            .Subscribe(value => notify.SetActive(value >= cost && cost != Values.Null))
            .AddTo(this.gameObject);

        InitData();
    }

    void InitData()
    {
        iconImage.sprite = GameData.Instance.GameItemSpriteMap[data.id];
    }

    public void InitializeWithData(int level)
    {
        if (_DataManager == null)
            _DataManager = DataManager.Instance;

        this.level = level;
        UpdateData(level);
    }

    public void UpdateData(int level)
    {
        cost = data.GetCost(level);

        notify.SetActive(_DataManager.Coin.Value >= cost && cost != Values.Null);

        levelBar.value = (float)level / data.maxLevel;

        this.level = level;
    }
}
