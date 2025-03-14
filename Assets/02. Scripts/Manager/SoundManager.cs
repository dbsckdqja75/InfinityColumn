﻿using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    bool isSfxMute = false;

    [SerializeField] float effectVolume = 0.8f;

    [SerializeField] MusicSource mainMusicSource, subMusicSource;
    [SerializeField] AudioSource effectAudioSource;

    [SerializeField] List<AudioClip> musicClipList;
    [SerializeField] List<AudioClip> soundClipList;

    Dictionary<string, AudioClip> audioClipList = new Dictionary<string, AudioClip>();

    protected override void Init()
    {
        effectAudioSource.volume = effectVolume;

        ResetAudioClipList();
    }

    void ResetAudioClipList()
    {
        audioClipList.Clear();

        foreach(AudioClip clip in musicClipList)
        {
            audioClipList.Add(clip.name, clip);
        }

        foreach(AudioClip clip in soundClipList)
        {
            audioClipList.Add(clip.name, clip);
        }
    }

    public void PlayMusic(string musicName, bool playFade = true)
    {
        if(audioClipList.ContainsKey(musicName))
        {
            if(playFade)
            {
                PlayMusicFade(audioClipList[musicName]);
            }
            else
            {
                PlayMusic(audioClipList[musicName]);
            }

            return;
        }

        Debug.LogWarningFormat("[Sound] 해당되는 이름의 오디오 파일이 목록에 없습니다! ({0})", musicName);
    }

    public void StopMusic(bool onStopFade = true)
    {
        if(mainMusicSource.IsPlaying())
        {
            if(onStopFade)
            {
                mainMusicSource.Stop();
            }
            else
            {
                mainMusicSource.StopForce();
            }
        }
        else
        {
            if(onStopFade)
            {
                subMusicSource.Stop();
            }
            else
            {
                subMusicSource.StopForce();
            }
        }
    }

    public void PlaySound(string soundName)
    {
        if(audioClipList.ContainsKey(soundName))
        {
            PlaySound(audioClipList[soundName]);
            return;
        }

        Debug.LogWarningFormat("[Sound] 해당되는 이름의 오디오 파일이 목록에 없습니다! ({0})", soundName);
    }

    public void PlaySound(string soundName, float volume)
    {
        if(audioClipList.ContainsKey(soundName))
        {
            PlaySound(audioClipList[soundName], false, volume);
            return;
        }

        Debug.LogWarningFormat("[Sound] 해당되는 이름의 오디오 파일이 목록에 없습니다! ({0})", soundName);
    }

    public void PlaySound(string soundName, Vector3 playPoint)
    {
        if(audioClipList.ContainsKey(soundName))
        {
            PlaySound(audioClipList[soundName], playPoint);
            return;
        }

        Debug.LogWarningFormat("[Sound] 해당되는 이름의 오디오 파일이 목록에 없습니다! ({0})", soundName);
    }

    void PlayMusic(AudioClip clip)
    {
        subMusicSource.StopForce();
        mainMusicSource.PlayForce(clip);
    }

    void PlayMusicFade(AudioClip clip)
    {
        if(mainMusicSource.GetPlayingMusicName() == clip.name)
        {
            subMusicSource.Stop();
            mainMusicSource.Play(clip);
            return;
        }
        else if(subMusicSource.GetPlayingMusicName() == clip.name)
        {
            mainMusicSource.Stop();
            subMusicSource.Play(clip);
            return;
        }

        if(mainMusicSource.IsPlaying())
        {
            mainMusicSource.Stop();
            subMusicSource.Play(clip);
        }
        else
        {
            subMusicSource.Stop();
            mainMusicSource.Play(clip);
        }
    }

    void PlaySound(AudioClip clip, bool playOnOneShot = false, float volume = 0)
    {
        if(!isSfxMute)
        {
            if(effectAudioSource.isPlaying || playOnOneShot)
            {
                effectAudioSource.PlayOneShot(clip, (volume > 0) ? volume : effectVolume);
            }
            else
            {
                effectAudioSource.volume = (volume > 0) ? volume : effectVolume;
                effectAudioSource.clip = clip;
                effectAudioSource.Play();
            }
        }
    }

    void PlaySound(AudioClip clip, Vector3 playPoint)
    {
        if(!isSfxMute)
        {
            AudioSource.PlayClipAtPoint(clip, playPoint, 1);
        }
    }

    public AudioClip GetSoundClip(string soundName)
    {
        if(!isSfxMute)
        {
            if(audioClipList.ContainsKey(soundName))
            {
                return audioClipList[soundName];
            }

            Debug.LogWarningFormat("[Sound] 해당되는 이름의 오디오 파일이 목록에 없습니다! ({0})", soundName);
        }

        return null;
    }

    public void MusicMute()
    {
        mainMusicSource.Mute();
        subMusicSource.Mute();
    }

    public void SfxMute()
    {
        isSfxMute = true;

        effectAudioSource.volume  = 0;
    }

    public void MusicUnmute()
    {
        mainMusicSource.Unmute();
        subMusicSource.Unmute();
    }

    public void SfxUnmute()
    {
        effectAudioSource.volume  = effectVolume;

        if(isSfxMute)
        {
            isSfxMute = false;
            SoundManager.Instance.PlaySound("DrumRoll1");
        }
    }
}
