using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;

public class UseItemElement : MonoBehaviour
{
    [SerializeField] UseItemData data;
    public UseItemData Data => data;

    [SerializeField] Toggle toggle;
    public Toggle Toggle => toggle;

    [SerializeField] Image iconImage;
    [SerializeField] GameObject notify;
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] GameObject equipObj;          // 장착중
    public bool IsEquip => equipObj.activeSelf;
    [SerializeField] Animator anim;
    // checkObj         *countObj 위에 올라갈거라 필요 x

    public System.Action<bool> OnEquip;
    public System.Action OnAdd;

    private int count;
    public int Count => count;
    private int adCount;
    public int AdCount => adCount;

    private static readonly int _Anim_Equip = Animator.StringToHash("Equip");

    private void Start()
    {
        InitData();
    }

    void InitData()
    {
        iconImage.sprite = GameData.Instance.GameItemSpriteMap[data.id];

        notify.SetActive(false);
        //DataManager.Instance.Coin
        //    .Subscribe(value => notify.SetActive(value >= data.cost))
        //    .AddTo(gameObject);

        equipObj.SetActive(false);

        OnAdd = AddItem;
    }

    public void InitializeWithData(int count, int adCount)
    {
        UpdateCount(count);
        UpdateAdCount(adCount);
    }

    public void UpdateCount(int count)
    {
        this.count = count;
        countText.text = count.ToString();
    }

    public void UpdateAdCount(int adCount)
    {
        this.adCount = adCount;
        // adCountText.text = $"AD {adCount}/{data.maxAdCount}"; -> UI
    }

    public void EquipItem(bool isEquip)
    {
        equipObj.SetActive(isEquip);
        OnEquip?.Invoke(isEquip);

        if(isEquip)
        {
            AudioManager.Instance.PlaySFX(ESfx.Equip);
            anim.SetTrigger(_Anim_Equip);
        }
    } 

    private void AddItem()
    {
        anim.SetTrigger(_Anim_Equip);
    }
}
