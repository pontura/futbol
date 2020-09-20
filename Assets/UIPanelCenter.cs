using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelCenter : MonoBehaviour
{
    public Text timerField;
    public int secs;

    void Start()
    {
        secs = Data.Instance.settings.totalTime;
        Loop();
        SetField();
    }
    void Loop()
    {
        if (Game.Instance.state == Game.states.PLAYING)
        {
            secs--;
            SetField();
        }
        Invoke("Loop", 1);
    }
    void SetField()
    {
       // int hours = secs / 3600;
        int minutes = (secs % 3600) / 60;
        int seconds = (secs % 3600) % 60;
        timerField.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
