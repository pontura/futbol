using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharactersPositions : MonoBehaviour
{
    public PositionsData[] all;

    [Serializable]
    public class PositionsData
    {
        public int id;
        public string name;
        public CharacterPositionData[] posData;
    }
    [Serializable]
    public class CharacterPositionData
    {
        public Character.types type;
        public Vector2 pos;
    }
    public PositionsData GetPositionsData(int id)
    {
        foreach(PositionsData p in all)
        {
            if (p.id == id)
                return p;
        }
        return null;
    }

}
