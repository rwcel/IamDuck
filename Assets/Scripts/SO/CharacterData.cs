using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObject/Character")]
public class CharacterData : ScriptableObject
{
    [BoxGroup("고유")] public EGameItem id;
    [BoxGroup("고유")] public ECharacter charID;
    
    [BoxGroup("형태")] public GameObject modelObj;
    [BoxGroup("형태")] public GameObject lobbyObj;
    [BoxGroup("형태")] public Sprite sprite;
    [BoxGroup("형태")] public Color themeColor = Color.white;

    [BoxGroup("해금")] public ECharacterCategory category;
    [BoxGroup("해금")] public int type;
    [BoxGroup("해금")] public int value;

    // 업데이트
    [BoxGroup("해금")] public bool updateReady;

    public string nameEntry => "Duck " + (int)(charID + 1);         // 1번부터 시작해야함
    [BoxGroup("로컬라이징")] public string unlockEntry;

}