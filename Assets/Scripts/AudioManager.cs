using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip[] danceSfx;

    [SerializeField] private float minDancestepInterval = 0.3f;
    [SerializeField] private float maxDancestepInterval = 0.6f;

    private float timeSinceLastDancestep;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic(backgroundMusic);
        timeSinceLastDancestep = -minDancestepInterval; // Initialize to allow immediate first play
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    public void PlayDanceSFX()
    {
        AudioClip footstepSound = danceSfx[Random.Range(0, danceSfx.Length)];
        sfxSource.PlayOneShot(footstepSound);
    }



    public void PlayDancestepSFX()
    {
        if (danceSfx.Length > 0 && Time.time - timeSinceLastDancestep >= Random.Range(minDancestepInterval, maxDancestepInterval))
        {
            AudioClip footstepSound = danceSfx[Random.Range(0, danceSfx.Length)];
            sfxSource.PlayOneShot(footstepSound);
            timeSinceLastDancestep = Time.time;
        }
    }
}
