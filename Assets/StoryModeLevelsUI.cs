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
        StoryModeData.LevelData data = StoryModeData.Instance.GetLevelActual();
        Data.Instance.stadiumData.SetActiveStadium(data.stadium_id);
        Data.Instance.matchData.secs = data.duration;
        Data.Instance.matchData.charactersPositions = data.charactersPositions;
        CharactersData.Instance.team2 = data.characters;
        Data.Instance.LoadLevel("GameIntro");
    }
}
