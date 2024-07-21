using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Music Chart")]
public class Chart : ScriptableObject
{
    public AudioClip clip;
    public float bpm;

    public Phrases[] phrases;
}

[System.Serializable]
public class Phrases
{
    public string name;
    public int endAfterBeat = 15;
    public bool playerTurn;
    public Note[] Notes;
}