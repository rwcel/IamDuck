using System;
using UnityEngine;

[System.Serializable]
public struct FTween
{
    public float value;
    public float time;
    public AnimationCurve curve;
}


public static class Values
{
    public static readonly string Key_Null = "";
    public static readonly string Key_Floor = "Floor";
    public static readonly string Key_Wall = "Wall";
    public static readonly string Key_Item = "Item";
    public static readonly string Key_Player = "Player";

    public static readonly string Key_Effect_GetItem = "GetItem";
    public static readonly string Key_Effect_Explosion = "Destroy";
    public static readonly string Key_Effect_Coin = "GetCoin";

    public static readonly string Key_Nickname_Null = "-";


    public static readonly int Layer_Floor = 1 << 6;

    public static readonly float FloorPerY = 4f;

    public static readonly int AllBlockPerCount = 5;

    public static readonly float ScreenSize_X = 1080;
    public static readonly float ScreenSize_Y = 1920;

    public static readonly float ScreenDivValue = 355.5f;
    public static readonly float MinScreenValue = 9.6f;

    public static readonly float Scale_Ingame_Model = 0.22f;           // 인게임만 해당

    public static readonly int Length_WallType = Enum.GetValues(typeof(EWallType)).Length;
    public static readonly int Length_FloorType = Enum.GetValues(typeof(EFloorType)).Length;

    public static readonly int Length_Floor = 3;
    public static readonly int Length_Obstacle = 1;
    public static readonly int Length_Item = 1;

    public static readonly int Length_Obstacle_Row = 8;

    public static readonly int Length_Attendance = 14;
    public static readonly int Length_RankPage = 30;
    public static readonly int Length_Rank = 99;
    public static readonly int Length_TopRank = 3;

    public static readonly int Point_Obstacle_InBot = 9;

    public static readonly float Limit_WallX = 4f;
    public static readonly float Limit_OutsideWallX = 5.5f;

    public static readonly int BaseTime = 60;

    public static readonly float Duration_Spring = 0.2f;
    public static readonly float Duration_Revive = 3f;

    public static readonly int Dist_SpringY = 4;

    public static readonly int Coin_PowerPotion = 10;

    public static readonly int Mission_Rewarded = -1;
    public static readonly int Mission_Reward_MaxValue = 90;

    public static readonly int Null = -1;

    public static readonly int Input_Limit_Coupon = 18;
    public static readonly int Input_Limit_Nickname = 12;

    public static readonly int MAX_Ad_Count = 3;

    // client_date
    // indate
    // update at
    // owner_indate
    public static readonly int Backend_Base = 4;

    public static readonly WaitForSeconds Delay1 = new WaitForSeconds(1f);
    public static readonly WaitForSeconds Delay01 = new WaitForSeconds(0.1f);
    public static readonly WaitForSeconds Delay002 = new WaitForSeconds(0.02f);
    public static readonly WaitForEndOfFrame DelayFrame = new WaitForEndOfFrame();
    public static readonly WaitForSecondsRealtime DelayReal1 = new WaitForSecondsRealtime(1f);

    public static readonly int _Anim_Close = Animator.StringToHash("Close");

    //public static string Path_DataManager = "Assets/Scripts/Manager/DataManager.cs";


    #region StringTable

    public static readonly string Prefs_Locale = "selected-locale";

    public static readonly string Local_Table_Common = "Common";
    public static readonly string Local_Entry_Recv = "Receive";
    public static readonly string Local_Entry_Close = "Close";
    public static readonly string Local_Entry_Message = "Message";
    public static readonly string Local_Entry_Confirm = "Confirm";
    public static readonly string Local_Entry_Cancel = "Cancel";
    // public static readonly string Local_Entry_Update = "Update";
    public static readonly string Local_Entry_NeedUpdate_Title = "Update Title";
    public static readonly string Local_Entry_NeedUpdate_Content = "Update Content";
    public static readonly string Local_Entry_Maintainance_Title = "Maintainance Title";
    public static readonly string Local_Entry_Maintainance_Content = "Maintainance Content";
    public static readonly string Local_Entry_Quit = "Quit";
    public static readonly string Local_Entry_ServerError = "Error Server";
    public static readonly string Local_Entry_GuestLogin = "Guest Login Confirm";
    public static readonly string Local_Entry_Logout = "Logout Confirm";
    public static readonly string Local_Entry_Signout = "Delete Account";

    public static readonly string Local_Table_Nickname = "Nickname";
    public static readonly string Local_Entry_Byte = "Byte";
    public static readonly string Local_Entry_IsMatch = "IsMatch";
    public static readonly string Local_Entry_Duplicate = "Duplicate";

    public static readonly string Local_Table_Intro = "IntroText";
    public static readonly string Local_Entry_WarningEssential = "Warning Essential";

    public static readonly string Local_Table_Setting = "Setting";
    public static readonly string Local_Entry_NullCoupon = "Null Coupon";
    public static readonly string Local_Entry_Copied = "Copied";
    public static readonly string Local_Entry_AccountExist = "Exist Account";
    public static readonly string Local_Entry_AllowPush = "Allow Push";

    public static readonly string Local_Table_Profile = "Profile";
    // public static readonly string Local_Entry_BestHeight = "BestHeight";
    public static readonly string Local_Name_BestHeight = "Score";

    public static readonly string Local_Table_Post = "Post";
    public static readonly string Local_Entry_Day = "Day";
    public static readonly string Local_Name_Limit = "limit";

    public static readonly string Local_Name_Mission = "mission_value";
    public static readonly string Local_Name_UseItem = "item_value";
    public static readonly string Local_Name_Upgrade = "upgrade_value";
    public static readonly string Local_Name_Unlock = "unlock_value";

    public static readonly string Local_Table_Manage = "Manage";
    public static readonly string Local_Entry_Equip = "Equip";
    public static readonly string Local_Entry_UnEquip = "UnEquip";
    public static readonly string Local_Entry_NeedMoney = "Need Money";
    public static readonly string Local_Entry_NotHave = "Not Have";
    public static readonly string Local_Entry_NotAds = "Not Ads";
    public static readonly string Local_Entry_MaxUpgrade = "Max Upgrade";

    public static readonly string Local_Table_InGame = "InGame";
    public static readonly string Local_Entry_Height = "Height";
    public static readonly string Local_Entry_Retry = "Retry Confirm";
    public static readonly string Local_Entry_Exit = "Exit Confirm";

    public static readonly string Local_Table_Character = "Character";
    public static readonly string Local_Entry_FailUnlock = "Fail Unlock";
    public static readonly string Local_Entry_AlreadyEquip = "Already Equip";
    public static readonly string Local_Entry_Update = "Update";
    public static readonly string Local_Entry_CharDesc = "Get Duck Desc";
    public static readonly string Local_Name_CharDesc = "duck_name";

    public static readonly string Local_Table_Shop = "Shop";
    public static readonly string Local_Name_ShopValue = "shop_value";
    public static readonly string Local_Entry_RemoveAd = "Remove Ad Content";
    public static readonly string Local_Entry_PurchaseSuccess = "Success";
    public static readonly string Local_Entry_PurchaseFail = "Fail";

    public static readonly string Local_Table_Mission = "Mission";
    public static readonly string Local_Entry_NextReward = "Click Next Reward";
    public static readonly string Local_Entry_FailMission = "Fail Mission";

    #endregion
}
