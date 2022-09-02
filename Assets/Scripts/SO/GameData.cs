using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObject/GameData")]
public class GameData : SerializedScriptableObject
{
    private const string FileDirectory = "Assets/Resources";
    private const string FilePath = "Assets/Resources/GameData.asset";
    private static GameData instance;
    public static GameData Instance
    {
        get
        {
            if (instance != null)
                return instance;
            instance = Resources.Load<GameData>("GameData");
#if UNITY_EDITOR
            if (instance == null)
            {
                if (!AssetDatabase.IsValidFolder(FileDirectory))
                {
                    AssetDatabase.CreateFolder("Assets", "Resources");
                }
                instance = AssetDatabase.LoadAssetAtPath<GameData>(FilePath);
                if (instance == null)
                {
                    instance = CreateInstance<GameData>();
                    AssetDatabase.CreateAsset(instance, FilePath);
                }
            }
#endif
            return instance;
        }
    }

    [Title("Application")]
    public bool isTestMode;
    public EStore Store;
    // Store ����

    [Title("������ ID")]
    public Dictionary<EGameItem, Sprite> GameItemSpriteMap;

    [Title("ĳ���� SO")]
    public CharacterData[] CharacterDatas;
    public GameObject CharacterModel(ECharacter type) => CharacterDatas[(int)type].modelObj;            // *Dictionary�� ����
    public Sprite CharacterSprite(int num) => CharacterDatas[num].sprite;                         // *Dictionary�� ����

    [Title("�̼�")]
    public MissionData[] MissionDatas;
    public MissionRewardData[] MissionRewardDatas;

    [Title("����")]
    public ShopData[] ShopDatas;

    [Title("��ư")]
    public FTween ButtonScaleTween;

}