using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObject/Character")]
public class CharacterData : ScriptableObject
{
    [BoxGroup("����")] public EGameItem id;
    [BoxGroup("����")] public ECharacter charID;
    
    [BoxGroup("����")] public GameObject modelObj;
    [BoxGroup("����")] public GameObject lobbyObj;
    [BoxGroup("����")] public Sprite sprite;
    [BoxGroup("����")] public Color themeColor = Color.white;

    [BoxGroup("�ر�")] public ECharacterCategory category;
    [BoxGroup("�ر�")] public int type;
    [BoxGroup("�ر�")] public int value;

    // ������Ʈ
    [BoxGroup("�ر�")] public bool updateReady;

    public string nameEntry => "Duck " + (int)(charID + 1);         // 1������ �����ؾ���
    [BoxGroup("���ö���¡")] public string unlockEntry;

}