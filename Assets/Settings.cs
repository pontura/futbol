﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Settings : MonoBehaviour
{
    public TeamSettings[] teamSettings;
    [Serializable]
    public class TeamSettings
    {
        public string name;
        public int designID;
        public Color color;
        public Color clothColorA;
        public Color clothColorB;
        public Color clothColorC;
        public Color clothColorD;
    }

    public Vector2 limits = new Vector2(40, 20);

    public float kickHard;
    public float kickHardAngle;

    public float kickSoft;
    public float kickSoftAngle;

    public float kickBaloon;
    public float kickBaloonAngle;

    public float speed;
    public float speedWithBall;
    public float speedDash;
}
