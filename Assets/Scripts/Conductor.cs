using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    AudioSource audioSource;

    public static float bpm;
    public static float crotchet;
    public static float offset;

    public static float songposition;
    public static float dsptimesong;

    public delegate void ConductorEvent(AudioClip _clip, float _bpm);
    public static ConductorEvent OnPlay, OnPause, OnStop;

    public static bool isPlaying;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        OnPlay += StartPlaying;
    }

    private void OnDestroy()
    {
        OnPlay -= StartPlaying;
    }

    public void StartPlaying(AudioClip _clip, float _bpm)
    {
        audioSource.clip = _clip;
        audioSource.Play();

        bpm = _bpm;
        crotchet = 60 / bpm;
        UpdateDspTime();

        isPlaying = true;
    }

    public void UpdateDspTime()
    {
        dsptimesong = (float)AudioSettings.dspTime;
    }

    private void Update()
    {
        if (isPlaying)
        songposition = (float)(AudioSettings.dspTime - dsptimesong) * audioSource.pitch - offset;
    }

}
