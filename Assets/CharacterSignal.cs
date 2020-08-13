using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSignal : MonoBehaviour
{

    public SpriteRenderer sr;

    public void Init(Color color)
    {
        sr.color = color;
    }
}
