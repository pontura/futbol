using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelCenter : MonoBehaviour
{
    public Text playerName;
    public Text timerField;
    public int secs;

   
    void Start()
    {
        playerName.text = "";
        secs = Data.Instance.settings.totalTime;
        Events.CharacterCatchBall += CharacterCatchBall;
        Events.OnBallKicked += OnBallKicked;
        Loop();
        SetField();
    }
    private void OnDestroy()
    {
        Events.CharacterCatchBall -= CharacterCatchBall;
        Events.OnBallKicked -= OnBallKicked;
    }
    void OnBallKicked()
    {
        Invoke("Reset", 1);
    }
    void CharacterCatchBall(Character character)
    {
        CancelInvoke();
        playerName.text = character.data.avatarName.ToUpper();
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
    void Reset()
    {
        playerName.text = "";
    }
}
