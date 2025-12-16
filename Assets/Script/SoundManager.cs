using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;  
    public AudioSource sfxSource;     

    [Header("Clips")]
    public AudioClip[] musicClips;   
    public AudioClip[] sfxClips;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SoundManager.Instance.PlayMusic(musicClips[0]); 
        musicSource.loop = true;   
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
        public void PauseMusic()
        {
            if (musicSource && musicSource.isPlaying)
                musicSource.Pause();
        }

        public void ResumeMusic()
        {
            if (musicSource && musicSource.clip)
                musicSource.UnPause();
        }

    // ================================
    // SFX
    // ================================
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }
}
