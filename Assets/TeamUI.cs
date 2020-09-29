using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamUI : MonoBehaviour
{
    public Image escudo;
    public Text scoreField;
    public Text teamNameField;
    public int score;
    public int teamID;

    void Start()
    {
        Events.OnGoal += OnGoal;
        Settings.TeamSettings settings = Data.Instance.settings.GetTeamSettings(teamID);
        teamNameField.text = settings.name;
        escudo.sprite = settings.escudo;
        SetField();
    }
    void OnDestroy()
    {
        Events.OnGoal -= OnGoal;
    }
    void OnGoal(int _teamID, Character character)
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
