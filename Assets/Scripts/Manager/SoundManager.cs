using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    public AudioSource musicSource;
    public AudioSource soundSource;
    public AudioClip[] musicClips; // 0 城市背景；1 战斗背景
    public AudioClip[] soundClips; // 0 战斗开始；1 战斗结束；2 战中打击
    public float lowPitchRange = .95f;
    public float hightPitchRange = 1.05f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSource.volume = DataManager.instance.configData.music;
        soundSource.volume = DataManager.instance.configData.sound;
    }

    public void PlayMusicByScene(string scene)
    {
        AudioClip clip = null;
        switch (scene)
        {
            case "City":
                clip = musicClips[0];
                break;
            case "Fight":
                clip = musicClips[1];
                break;
            default:
                Debug.LogError("未知的场景");
                break;
        }
        PlayMusic(clip);
    }

    public void PlaySoundByScene(string scene)
    {
        AudioClip clip = null;
        switch (scene)
        {
            case "FightStart":
                clip = soundClips[0];
                break;
            case "FightEnd":
                clip = soundClips[1];
                break;
            case "FightFit":
                clip = soundClips[2];
                break;
            default:
                Debug.LogError("未知的场景");
                break;
        }
        PlaySound(clip);
    }

    public void PlayMusic(AudioClip clip, bool isLoop = true)
    {
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = isLoop;
        musicSource.Play();
    }

    public void PlaySound(AudioClip clip, bool isLoop = false)
    {
        soundSource.Stop();
        soundSource.clip = clip;
        soundSource.loop = isLoop;
        soundSource.Play();
    }

    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        soundSource.clip = clips[randomIndex];
        soundSource.Play();
    }
}
