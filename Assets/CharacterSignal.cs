using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSignal : MonoBehaviour
{

    public SpriteRenderer[] all;

    public void Init(Color color)
    {
        foreach(SpriteRenderer sr in all)
            sr.color = color;
    }
}
