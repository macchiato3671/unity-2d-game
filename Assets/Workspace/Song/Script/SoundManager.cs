using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager inst;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    void Start()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
            SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume", 0.6f));
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.6f));
            bgmSource.outputAudioMixerGroup = bgmGroup;
            sfxSource.outputAudioMixerGroup = sfxGroup;
        }
        else Destroy(gameObject);
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        
        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void PlayTitleBGM()
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/BGM/title");
        if (clip != null) PlayBGM(clip);
        else Debug.Log("TitleBGM 재생 실패");
    }

    public void PlayBaseBGM()
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/BGM/base");
        if (clip != null) PlayBGM(clip);
        else Debug.Log("BaseBGM 재생 실패");
    }

    public void PlayAdventureBGM()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/BGM/adventure");
        if (clips.Length > 0)
        {
            AudioClip randomClip = clips[Random.Range(0, clips.Length)];
            PlayBGM(randomClip);
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PauseBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Pause();
    }

    public void ResumeBGM()
    {
        if (!bgmSource.isPlaying)
            bgmSource.UnPause();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        
        sfxSource.PlayOneShot(clip,volume);
    }

    public void SetBGMVolume(float value)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("BGMVolume", value);
        Debug.Log($"배경음 크기 설정됨 : {value}");
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
        Debug.Log($"효과음 크기 설정됨 : {value}");
    }
}
