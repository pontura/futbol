using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StoryModeData : MonoBehaviour
{
    public static StoryModeData mInstance;
    public static StoryModeData Instance
    {
        get { return mInstance; }
    }

    public TextAsset file;

    public AllData all;
    [Serializable]
    public class AllData
    {
        public LevelData[] levels;
    }

    [Serializable]
    public class LevelData
    {
        public string name;
        public int stadium_id;
        public int duration;
        public int[] charactersPositions;
        public List<int> characters;
    }

    public int id;

    void Awake()
    {
        if (mInstance != null)
            Destroy(gameObject);
        else
        {
            mInstance = this;
            all = JsonUtility.FromJson<AllData>(file.text);
            DontDestroyOnLoad(gameObject);
        }
    }
    public LevelData GetLevelActual()
    {
        return all.levels[id];
    }
}
