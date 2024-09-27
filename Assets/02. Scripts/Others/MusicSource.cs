using System.Collections;
using UnityEngine;

public class MusicSource : MonoBehaviour
{
    bool isPlaying = false;
    
    bool isMute;

    [SerializeField] bool playOnStart;
    [SerializeField] bool playLoop;
    [SerializeField] bool fadeStart;
    
    [SerializeField] float fadeVolumeTimeSeconds = 1f;
    [SerializeField] float musicVolume = 0.4f;

    AudioSource audioSource;

    Coroutine musicFadeCoroutine;

    void Awake() 
    {
        Init();
    }

    void Init()
    {
        audioSource = this.GetComponent<AudioSource>();
        audioSource.loop = playLoop;
        audioSource.volume = playOnStart ? musicVolume : 0;
    }

    void Start()
    {
        PlayOnStart();
    }

    void PlayOnStart()
    {
        if(playOnStart)
        {
            if(fadeStart)
            {
                audioSource.volume = 0;
                
                Play(audioSource.clip);
            }
            else
            {
                audioSource.Play();

                isPlaying = true;
            }
        }
    }

    public void Play(AudioClip musicClip)
    {
        musicFadeCoroutine?.Stop(this);
        musicFadeCoroutine = FadeInMusic(musicClip).Start(this);
    }

    public void PlayForce(AudioClip musicClip)
    {
        musicFadeCoroutine?.Stop(this);

        audioSource.Stop();
        audioSource.clip = musicClip;
        audioSource.Play();

        isPlaying = true;
    }

    public void Stop()
    {
        musicFadeCoroutine?.Stop(this);
        musicFadeCoroutine = FadeOutMusic().Start(this);
    }

    public void StopForce()
    {
        musicFadeCoroutine?.Stop(this);
        audioSource.Stop();

        isPlaying = false;
    }

    public void Mute()
    {
        isMute = true;

        musicFadeCoroutine?.Stop(this);
        audioSource.volume = 0;
    }

    public void Unmute()
    {
        isMute = false;

        if(isPlaying)
        {
            audioSource.volume = musicVolume;
        }
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    IEnumerator FadeInMusic(AudioClip musicClip)
    {
        PlayForce(musicClip);

        while(audioSource.volume < musicVolume && !isMute)
        {
            audioSource.volume += (musicVolume / fadeVolumeTimeSeconds) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        audioSource.volume = (isMute ? 0 : musicVolume);

        yield break;
    }
    
    IEnumerator FadeOutMusic()
    {
        while(audioSource.volume > 0 && !isMute)
        {
            audioSource.volume -= (musicVolume / fadeVolumeTimeSeconds) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        audioSource.volume = 0;

        StopForce();

        yield break;
    }
}
