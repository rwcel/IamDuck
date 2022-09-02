using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Audio")]
public class AudioData : ScriptableObject
{
    [System.Serializable]
    public struct FBgmSound
    {
        [SerializeField] AudioClip clip;
        [SerializeField] EBgm id;
        [Range(0f, 1f)]
        [SerializeField] float volume;

        public AudioClip Clip => clip;
        public EBgm Id => id;
        public float Volume => volume;
    }

    [System.Serializable]
    public struct FSfxSound
    {
        [SerializeField] AudioClip clip;
        [SerializeField] ESfx id;

        public AudioClip Clip => clip;
        public ESfx Id => id;
    }


    [SerializeField]
    FBgmSound[] bgms;

    [SerializeField]
    FSfxSound[] sfxs;

    Dictionary<EBgm, FBgmSound> bgmMap = new Dictionary<EBgm, FBgmSound>();
    Dictionary<ESfx, AudioClip> sfxMap = new Dictionary<ESfx, AudioClip>();

    public void GenerateSounds()
    {
        // 그대로라서 따로 처 할 필요 없음
        if (bgmMap.Count > 0)
            return;

        foreach (var sound in bgms)
        {
            bgmMap.Add(sound.Id, sound);
        }
        foreach (var sound in sfxs)
        {
            sfxMap.Add(sound.Id, sound.Clip);
        }
    }

    public FBgmSound GetBgm(EBgm id)
    {
        if (bgmMap.Count == 0)
        {
            GenerateSounds();
        }

        return bgmMap[id];
    }

    public AudioClip GetBgmClip(EBgm id)
    {
        if (bgmMap.Count == 0)
        {
            GenerateSounds();
        }

        return bgmMap[id].Clip;
    }

    public AudioClip GetSFX(ESfx id)
    {
        if(sfxMap.Count == 0)
        {
            GenerateSounds();
        }

        if (!sfxMap.ContainsKey(id))
            return null;

        return sfxMap[id];
    }
}
