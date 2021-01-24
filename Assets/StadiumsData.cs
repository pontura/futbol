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
        public float size_x;
        public float size_y;
        public int totalPlayers;
        public int charactersPositions_id;
        public GameObject stadiumAsset;
    }
    void Awake()
    {
        active = all[id];
    }
    public void SetActiveStadium(int id)
    {
        active = all[id];
        this.id = id;
    }
}
