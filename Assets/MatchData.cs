using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchData : MonoBehaviour
{
    public int secs;
    public Vector2 score;
    public int penaltyGoalKeeperTeamID;
    public int totalPlayers;

    void Start()
    {
        totalPlayers = 1;
        secs = Data.Instance.settings.totalTime;
        Loop();
    }
    public void OnGoal(int _teamID)
    {
        if (_teamID == 1)
            score.x++;
        else
            score.y++;
    }
    void Loop()
    {
        if (Game.Instance != null && Game.Instance.state == Game.states.PLAYING)
            secs--;
        Invoke("Loop", 1);
    }

}
