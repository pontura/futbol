using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelCenter : MonoBehaviour
{
    public Text timerField;

    void Start()
    {
        Loop();
        SetField();
    }
    void Loop()
    {
        if (Game.Instance.state == Game.states.PLAYING)
        {
            SetField();
        }
        Invoke("Loop", 1);
    }
    void SetField()
    {
        // int hours = secs / 3600;
        int secs = Data.Instance.matchData.secs;
        int minutes = (secs % 3600) / 60;
        int seconds = (secs % 3600) % 60;
        timerField.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
