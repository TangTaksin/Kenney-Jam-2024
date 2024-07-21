using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum input
{
    Top, Bottom, Left, Right
}

[System.Serializable]
public class Note
{
    public float onBeat;
    public input input;

}
