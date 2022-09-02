using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [BoxGroup("공통")]
    [SerializeField] AudioData data;
    [BoxGroup("배경음")]
    [SerializeField] AudioSource bgmSource;
    [BoxGroup("배경음")]
    [SerializeField] AudioMixerGroup bgmGroup;
    [BoxGroup("효과음")]
    [SerializeField] AudioSource[] sfxSources;
    [BoxGroup("효과음")]
    [SerializeField] AudioMixerGroup sfxGroup;

    private bool onoffBGM;
    public bool OnOffBGM => onoffBGM;
    private static readonly string _Key_SwitchBGM = "SwitchBGM";
    private bool onoffSFX;
    public bool OnOffSFX => onoffSFX;
    private static readonly string _Key_SwitchSFX = "SwitchSFX";

    AudioSource loopSource;
    BackendManager _Server;


    protected override void AwakeInstance()
    {
        _Server = BackendManager.Instance;

        data.GenerateSounds();

        // **이러면 리스타트에 여러번 실행할 것 같음
        // _Server.OnGameLoad += LoadData();
        
    }

    protected override void DestroyInstance()
    {
        
    }

    /// <summary>
    /// PlayerPrefs
    /// </summary>
    public void LoadData()
    {
        onoffBGM = _Server.PrefsHasKey(_Key_SwitchBGM)
                            ? _Server.PrefsGetInt(_Key_SwitchBGM) == 0 ? false : true
                            : true;
        bgmGroup.audioMixer.SetFloat("BGM", onoffBGM ? 0 : -80);

        onoffSFX = _Server.PrefsHasKey(_Key_SwitchSFX)
                            ? _Server.PrefsGetInt(_Key_SwitchSFX) == 0 ? false : true
                            : true;
        sfxGroup.audioMixer.SetFloat("SFX", onoffSFX ? 0 : -80);
    }

    public void PlayBGM(EBgm id)
    {
        var sound = data.GetBgm(id);
        bgmSource.PlayOneShot(sound.Clip);
        // bgmSource.volume = sound.Volume;
    }

    public void PauseBGM(bool isPause)
    {
        Debug.Log($"BGM Pause : {isPause}");

        if (isPause)
            bgmSource.Pause();
        else
            bgmSource.UnPause();
    }

    public void PlaySceneBGM(EScene scene)
    {
        switch (scene)
        {
            case EScene.CI:
                bgmSource.Stop();
                break;
            case EScene.Intro:
                bgmSource.clip = data.GetBgmClip(EBgm.OutGame);
                bgmSource.Play();
                break;
            case EScene.OutGame:
                if (bgmSource.clip == data.GetBgmClip(EBgm.OutGame))
                {
                    Debug.Log("이미 재생중");
                    return;
                }
                bgmSource.clip = data.GetBgmClip(EBgm.OutGame);
                bgmSource.Play();
                break;
            case EScene.InGame:
                bgmSource.clip = data.GetBgmClip(EBgm.InGame);
                bgmSource.Play();
                break;
        }
    }

    public void PlaySFX(ESfx id)
    {
        // Debug.Log(id);
        var source = GetEmptySFX();
        if (source != null && data.GetSFX(id) != null)
            source.PlayOneShot(data.GetSFX(id));
        else 
            Debug.Log("사운드 없음");
    }

    public void PlayLoopSFX(ESfx id)
    {
        var source = GetEmptySFX();
        if (source != null)
        {
            loopSource = source;
            loopSource.clip = data.GetSFX(id);
            loopSource.Play();
            loopSource.loop = true;
        }
    }

    public void ClearLoop()
    {
        if (loopSource == null)
            return;

        loopSource.loop = false;
        loopSource.clip = null;
        loopSource = null;
    }

    AudioSource GetEmptySFX()
    {
        for (int i = 0, length = sfxSources.Length; i < length; i++)
        {
            if (!sfxSources[i].isPlaying)
                return sfxSources[i];
        }

        return null;
    }

    public bool SwitchBGM()
    {
        onoffBGM = !onoffBGM;
        bgmGroup.audioMixer.SetFloat("BGM", onoffBGM ? 0 : -80);
        Debug.Log("BGM : " + onoffBGM);
        //bgmMixer.SetFloat();
        SaveBGM(onoffBGM);

        return onoffBGM;
    }

    public bool SwitchSFX()
    {
        onoffSFX = !onoffSFX;
        sfxGroup.audioMixer.SetFloat("SFX", onoffSFX ? 0 : -80);
        //sfxMixer.SetFloat("SFX", onoffSFX ? 0 : -80);
        SaveSFX(onoffSFX);

        return onoffSFX;
    }

    private void SaveBGM(bool onoff)
    {
        _Server.PrefsSetInt(_Key_SwitchBGM, onoff ? 1 : 0);
    }

    private void SaveSFX(bool onoff)
    {
        _Server.PrefsSetInt(_Key_SwitchSFX, onoff ? 1 : 0);
    }
}
