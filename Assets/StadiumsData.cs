using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StadiumsData : MonoBehaviour
{
    public int id;
    public StadiumData active;
    public StadiumData[] all;

    [Serializable]
    public class StadiumData
    {
        public string name;
        public string sceneName;
        public float size_x;
        public float size_y;
    }
    void Awake()
    {
        active = all[id];
    }
}
