﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFloorSignal : MonoBehaviour
{
    public SpriteRenderer spriteRender;
    Color color;

    public void Init(Color color)
    {
        this.color = color;
        SetOn(false);
    }

    public void SetOn(bool isOn)
    {
        if (isOn)
            color.a = 1;
        else
            color.a = 0.6f;

        spriteRender.color = color;
    }
}
