using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsButton : MonoBehaviour
{
    public Text field;
    public int id;
    StoryModeLevelsUI ui;

    public void Init(StoryModeLevelsUI ui, StoryModeData.LevelData levelData, int id)
    {
        this.ui = ui;
        field.text = levelData.name;
    }
    public void Clicked()
    {
        ui.Clicked( id );
    }
}
