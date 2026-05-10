using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [Header("Audio clip")]
    public AudioClip collectBrickSfx;
    public AudioClip winSfx;
    public AudioClip placeBrickSfx;
    public AudioClip lostSfx;
    public AudioClip waitingMusic;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip, float pitch = 1.0f)
    {
        if (clip == null) 
        {
            return;
        }
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return; 
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.loop = true; 
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
