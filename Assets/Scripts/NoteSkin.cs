using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Note Skin")]
public class NoteSkin : ScriptableObject
{
    public Sprite Top, Bottom, Left, Right;
    public Color color;

    public Sprite GetDirectionSprite(input _input = input.Top)
    {
        Sprite sprite = null;

        switch (_input)
        {
            case input.Top:
                sprite = Top;
                break;
            case input.Bottom:
                sprite = Bottom;
                break;
            case input.Left:
                sprite = Left;
                break;
            case input.Right:
                sprite = Right;
                break;
        }

        return sprite;
    }
}
