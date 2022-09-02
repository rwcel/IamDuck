using Sirenix.OdinInspector;

public enum EScene
{
    CI = 0,
    Intro = 1,
    OutGame = 2,
    InGame = 3,
}

/// <summary>
/// ** 내용이 달라야하나? 
/// (ex. 캣점프의 순간이동. 그런 경우면 하나 더 만들어야)
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
    Time,                   // 사용안함
    [LabelText("청양고추")]
    PowerPotion,            // PowerPotion 
    [LabelText("바람개비")]
    Spring,
    [LabelText("자석")]
    Magnet,
    [LabelText("코인")]
    Coin,
}
[System.Flags]
public enum EInGameItemBit
{
    Time = 1 << 0,              // 미사용
    PowerPotion = 1 << 1,
    Spring = 1 << 2,
    Magnet = 1 << 3,
    Coin = 1 << 4,
}

public enum EUpgradeType
{
    ItemAppear,
    PowerPotion,
    Spring,         // *위치 변경
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
    DailyCheckChar = 410,       // 출석 보상
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
    Item,                       // 우편 보낼 저장 용도임
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
    ResultHeight,           // 도달
    AccuHeight,             // 누적 층
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
    UseGoods,           // 재화 사용
    HaveGoods,          // 재화 보유
    Attendance,         // 출석
    Payment,             // 결제 금액
    Destroy,                // 적 처치
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
    [LabelText("일반 버튼 터치")]
    Touch,
    [LabelText("비활성화 버튼 터치")]
    Disable,
    [LabelText("보상 획득/구매")]
    Reward,
    [LabelText("아이템 구매")]
    ItemBuy,
    [LabelText("아이템 장착")]
    Equip,
    [LabelText("업그레이드")]
    Upgrade,
    [LabelText("출석 보상")]
    Attendance,
    [LabelText("오리 해금")]
    Unlock,
    [LabelText("점프")]
    Jump,
    [LabelText("더블 점프")]
    DoubleJump,
    [LabelText("사망")]
    Death,
    [LabelText("부활")]
    Revive,
    [LabelText("청양고추")]
    PowerPotion,            // PowerPotion 
    [LabelText("바람개비")]
    Spring,
    [LabelText("자석")]
    Magnet,
    [LabelText("아이템 획득")]
    ItemAdd,
    [LabelText("코인")]
    Coin,
    [LabelText("결과")]
    Result,
    [LabelText("최고기록 달성")]
    BestResult,
    [LabelText("게임 시작")]
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