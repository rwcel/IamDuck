using Sirenix.OdinInspector;

public enum EScene
{
    CI = 0,
    Intro = 1,
    OutGame = 2,
    InGame = 3,
}

/// <summary>
/// ** ������ �޶���ϳ�? 
/// (ex. Ĺ������ �����̵�. �׷� ���� �ϳ� �� ������)
/// </summary>
public enum EWallType
{
    Normal = 0,
    Deco1,
    Deco2,
    Deco3
}

public enum EFloorType
{
    None = -1,
    Normal,
    Swamp,
    Ice,
    Bomb,      // Bomb
}

public enum EObstacleType
{
    Jammer,
    StaticGear,
    DynamicGear,
    Missile,
    Laser,
}

public enum EObstacleMoveType
{
    Static,
    UniDynamic,
    BiDynamic,
}

public enum EIngameItem
{
    Time,                   // ������
    [LabelText("û�����")]
    PowerPotion,            // PowerPotion 
    [LabelText("�ٶ�����")]
    Spring,
    [LabelText("�ڼ�")]
    Magnet,
    [LabelText("����")]
    Coin,
}
[System.Flags]
public enum EInGameItemBit
{
    Time = 1 << 0,              // �̻��
    PowerPotion = 1 << 1,
    Spring = 1 << 2,
    Magnet = 1 << 3,
    Coin = 1 << 4,
}

public enum EUpgradeType
{
    ItemAppear,
    PowerPotion,
    Spring,         // *��ġ ����
    Magnet,
    Resist,
}

public enum EUseItemType
{
    StartBoost1,
    StartBoost2,
    BonusCoin,
    Continue,
    IgnoreBomb,
    ObstacleAppear,         // *Upgrade -> UseItem
}
[System.Flags]
public enum EUseItemBit
{
    StartBoost1 = 1 << 0,
    StartBoost2 = 1 << 1,
    BonusCoin = 1 << 2,
    Continue = 1 << 3,
    IgnoreBomb = 1 << 4,
    ObstacleAppear = 1 << 5,
}

public enum EInGamePopup
{
    Pause,
    Continue,
    GameOver,
}

public enum EOutGamePopup
{
    Unlock,
    Attendance,
    Mission,
    Shop,
    Mail,
    Profile,
    Rank,
    Setting,
    Notice,
    LinkAccount,
}

public enum EPlayerState
{
    Run,
    Jump,
    DoubleJump,
    Hit,
    Die,
    Revive,
    Fever,          // Invisible
}

public enum ECSV
{
    DailyCheck,
    Item,
}

public enum EGameItem
{
    Coin = 101,
    CashDia,
    FreeDia,
    Dia,
    Gauge,
    StartBoost1 = 201,
    StartBoost2,
    BonusCoin,
    Continue,
    IgnoreBomb,
    ObstacleAppear,
    ItemAppear = 301,
    PowerPotion,
    Spring,
    Magnet,
    Resist,
    NormalChar = 401,           // ** 400 -> 401
    DailyCheckChar = 410,       // �⼮ ����
    RemoveAd = 601,
    DoubleCoin = 602,
}

public enum ELogin
{
    Google,
    Facebook,
    Guest,
}

public enum EChart 
{
    Item,                       // ���� ���� ���� �뵵��
    Attendance,
    DailyMission,
    MissionReward,
    Character,
    Shop,
}

public enum EGoods
{
    Coin,
    CashDia,
    FreeDia,
    Dia             // Cash + Free
}

public enum EMissionCategory
{
    Gameplay,
    AddInGameItem,
    UseItem,
    AddItem,
}

public enum EGameplay 
{
    GamePlay,
    ResultHeight,           // ����
    AccuHeight,             // ���� ��
}

public enum EElementState
{   // Sorting
    Clear,
    Progress,
    Reward,
}

public enum EHomeNotify
{
    Mission,
    Manage,
    // Character,
    // Mail,
    // Shop
}

public enum ECharacter
{
    Normal,
    Pink,
    Green,
    Ribbon,
    Angry,
    Mohican,
    Twinkle,
    Doll,
    Chicken,
    Slime,
    Pajama,
    Pig,
    Tiger,
    Short_Hair,
    Naval,
    Paleolithic,
    Stone,
    Children,
    Agent,
    Ghost,
    Robot
}

public enum ECharacterCategory
{
    None = -1,
    GamePlay,
    InGameItem,         
    UseGoods,           // ��ȭ ���
    HaveGoods,          // ��ȭ ����
    Attendance,         // �⼮
    Payment,             // ���� �ݾ�
    Destroy,                // �� óġ
}

public enum EInputType
{
    CreateNickname,
    ModifyNickname,
    Coupon,
}

public enum EShopType
{
    Ad,
    Buy,
    Exchange,
    Weekly,
    Period,
    Limit,
}

public enum ELanguage
{
    en,
    ko
}

public enum ETransition
{
    Dissolve,
    Vertical,
}

public enum EAds
{
    StartBoost1,
    StartBoost2,
    BonusCoin,
    Continue,
    IgnoreBomb,
    ObstacleAppear,
    Coin,
}

public enum EBgm
{
    OutGame,
    InGame,
}

public enum ESfx
{
    [LabelText("�Ϲ� ��ư ��ġ")]
    Touch,
    [LabelText("��Ȱ��ȭ ��ư ��ġ")]
    Disable,
    [LabelText("���� ȹ��/����")]
    Reward,
    [LabelText("������ ����")]
    ItemBuy,
    [LabelText("������ ����")]
    Equip,
    [LabelText("���׷��̵�")]
    Upgrade,
    [LabelText("�⼮ ����")]
    Attendance,
    [LabelText("���� �ر�")]
    Unlock,
    [LabelText("����")]
    Jump,
    [LabelText("���� ����")]
    DoubleJump,
    [LabelText("���")]
    Death,
    [LabelText("��Ȱ")]
    Revive,
    [LabelText("û�����")]
    PowerPotion,            // PowerPotion 
    [LabelText("�ٶ�����")]
    Spring,
    [LabelText("�ڼ�")]
    Magnet,
    [LabelText("������ ȹ��")]
    ItemAdd,
    [LabelText("����")]
    Coin,
    [LabelText("���")]
    Result,
    [LabelText("�ְ��� �޼�")]
    BestResult,
    [LabelText("���� ����")]
    GameStart,
}

public enum ELoginPopup
{
    Maintainance,
    Update,
    CantLogin,
}

public enum EStore
{
    Google,
    Onestore,
}

public enum EPostType
{
    Normal,
    Package,
}