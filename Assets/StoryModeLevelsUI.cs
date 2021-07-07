using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryModeLevelsUI : MonoBehaviour
{
    public Transform container;
    public LevelsButton button;

    void Start()
    {
        Utils.RemoveAllChildsIn(container);
        int id = 0;
        foreach(StoryModeData.LevelData data in StoryModeData.Instance.all.levels)
        {
            LevelsButton b = Instantiate(button, container);
            b.Init(this, data, id);
            id++;
        }
    }  
    public void Clicked(int id)
    {
        StoryModeData.Instance.id = id;
        Data.Instance.LoadLevel("Game");
    }
}
