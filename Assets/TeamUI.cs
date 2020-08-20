using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamUI : MonoBehaviour
{
    public Text scoreField;
    public Text teamNameField;
    public int score;
    public int teamID;

    void Start()
    {
        Events.OnGoal += OnGoal;
        SetField();
    }
    void OnDestroy()
    {
        Events.OnGoal -= OnGoal;
    }
    void OnGoal(int _teamID)
    {
        if (teamID == _teamID)
        {
            score++;
            SetField();
        }
    }
    void SetField()
    {
        scoreField.text = "0" + score.ToString();
    }
}
