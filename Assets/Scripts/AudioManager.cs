using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip[] bgmClip;
    public float bgmVolume;
    public bool bgmVolumeMute;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;
    public Bgm bgm;

    public enum Bgm { title, Home, Shop, Battle, None }

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public bool sfxVolumeMute;
    public int channels;
    AudioSource[] sfxPlayer;
    int channelIndex;

    public enum Sfx { Click ,Sword ,Masic }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip[0];
        bgmPlayer.loop = true;
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        //효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayer = new AudioSource[channels];

        for (int index = 0; index < channels; index++)
        {
            sfxPlayer[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayer[index].playOnAwake = false;
            sfxPlayer[index].bypassListenerEffects = true;
            sfxPlayer[index].volume = sfxVolume;
        }
    }

    public void PlayBgm(Bgm bgm, bool isPlay)
    {
        if (this.bgm == bgm)
            return;

        if (isPlay)
        {
            this.bgm = bgm;
            bgmPlayer.clip = bgmClip[(int)bgm];
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int index = 0; index < channels; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayer.Length;

            if (sfxPlayer[loopIndex].isPlaying)
            {
                continue;
            }

            int ranIndex = 0;

            channelIndex = loopIndex;
            sfxPlayer[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayer[loopIndex].Play();
            break;
        }

    }

    public void SetBgmVolume(float volume)
    {
        bgmPlayer.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        for (int i = 0; i < channels; i++)
        {
            sfxPlayer[i].volume = volume;
        }
    }
}
