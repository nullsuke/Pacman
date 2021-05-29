using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager
{
    private readonly AudioSource bgmAudioSource;
    private readonly AudioSource seAudioSource;
    private readonly AudioClip[] allAudioPool;
    private readonly Dictionary<string, AudioClip> bgmPool = 
        new Dictionary<string, AudioClip>();
    private readonly Dictionary<string, AudioClip> sePool = 
        new Dictionary<string, AudioClip>();

    public static SoundManager Instance { get; } = new SoundManager();

    //BGMのAudioclipをDictionaryに追加。
    public void LoadBGM(string key, string name)
    {
        if (bgmPool.ContainsKey(key)) return;

        var audio = GetAudioClip(name);

        bgmPool.Add(key, audio);
    }

    //SEのAudioclipをDictionaryに追加。
    public void LoadSE(string key, string name)
    {
        if (sePool.ContainsKey(key)) return;

        var audio = GetAudioClip(name);

        sePool.Add(key, audio);
    }

    //BGMの再生。
    public void PlayBGM(string name, float volume = 1)
    {
        if (bgmPool[name] == null) throw new Exception("not found");

        bgmAudioSource.clip = bgmPool[name];
        bgmAudioSource.loop = true;
        bgmAudioSource.volume = Mathf.Clamp01(volume);
        bgmAudioSource.Play();
    }

    //SEの再生。
    public void PlaySE(string name, float volume = 1)
    {
        if (sePool[name] == null) throw new Exception("not found");

        volume = Mathf.Clamp01(volume);
        seAudioSource.PlayOneShot(sePool[name], volume);
    }

    //BGMの停止。
    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }

    //BGMの一時停止。
    public void PauseBGM()
    {
        if (bgmAudioSource.isPlaying) bgmAudioSource.Pause();
        else bgmAudioSource.UnPause();
    }

    //SEの停止。
    public void StopSE()
    {
        seAudioSource.Stop();
    }

    private SoundManager()
    {
        allAudioPool = Resources.LoadAll<AudioClip>("Audio");

        var obj = new GameObject("SoundManager");
        bgmAudioSource = obj.AddComponent<AudioSource>();
        seAudioSource = obj.AddComponent<AudioSource>();
        UnityEngine.Object.DontDestroyOnLoad(obj);
    }

    //DictionaryからAudioClipを取得。
    private AudioClip GetAudioClip(string name)
    {
        var audio = allAudioPool.FirstOrDefault(a => a.name == name);

        if (audio == null) throw new Exception("not found");

        return audio;
    }
}
