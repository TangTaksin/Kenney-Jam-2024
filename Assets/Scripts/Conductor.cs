using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conductor : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource musicSource;
    public float songBpm;
    public float firstBeatOffset;

    [Header("Note Settings")]
    public GameObject[] notePrefabs; // Array of note prefabs
    public Transform[] spawnPoints; // Array of spawn points for notes
    public List<float> noteTimings; // List of note timings in beats

    private float secPerBeat;
    private float songPosition;
    private float dspSongTime;
    private int nextIndex = 0; // Next note to spawn

    void Start()
    {
        secPerBeat = 60f / songBpm;
        dspSongTime = (float)AudioSettings.dspTime;
        musicSource.Play();
    }

    void Update()
    {
        UpdateSongPosition();

        if (nextIndex < noteTimings.Count && songPosition / secPerBeat >= noteTimings[nextIndex])
        {
            SpawnNote();
            nextIndex++;
        }
    }

    private void UpdateSongPosition()
    {
        songPosition = (float)(AudioSettings.dspTime - dspSongTime) - firstBeatOffset;
    }

    private void SpawnNote()
    {
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);
        int notePrefabIndex = Random.Range(0, notePrefabs.Length);
        Instantiate(notePrefabs[notePrefabIndex], spawnPoints[spawnPointIndex].position, Quaternion.identity);
    }
}
