using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamUI : MonoBehaviour
{
    public Image escudo;
    public Text scoreField;
    public Text teamNameField;
    public int teamID;

    void Start()
    {
        Events.OnGoal += OnGoal;
        Settings.TeamSettings settings = Data.Instance.settings.GetTeamSettings(teamID);
        teamNameField.text = settings.name_abr;
        escudo.sprite = settings.escudo;
        SetField();
    }
    void OnDestroy()
    {
        Events.OnGoal -= OnGoal;
    }
    void OnGoal(int _teamID, Character character)
    {
        SetField();
    }
    void SetField()
    {
        int score = 0;
        if (teamID == 1)
            score = (int)Data.Instance.matchData.score.x;
        else
            score = (int)Data.Instance.matchData.score.y;
        scoreField.text = "0" + score.ToString();
    }
}
