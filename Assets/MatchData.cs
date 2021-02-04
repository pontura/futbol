using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchData : MonoBehaviour
{
    public int secs;
    public Vector2 score;
    public int penaltyGoalKeeperTeamID;
    public int totalPlayers;
    public int lastGoalBy;
    public int totalCharacters;
    public int[] charactersPositions;

    void Start()
    {
        totalPlayers = 1;
        secs = Data.Instance.settings.totalTime;
        Events.GameInit += GameInit;
    }
    private void OnDestroy()
    {
        Events.GameInit -= GameInit;
    }
    void GameInit()
    {
        CancelInvoke();
        if (secs == Data.Instance.settings.totalTime)
            Reset();
        Loop();
    }
    private void Reset()
    {
        score = Vector2.zero;
        lastGoalBy = 0;
        penaltyGoalKeeperTeamID = 0;
    }
    public void OnGoal(int _teamID)
    {
        if (_teamID == 1)
            score.x++;
        else
            score.y++;
        lastGoalBy = _teamID;
    }
    void Loop()
    {
        if (Game.Instance != null && Game.Instance.state == Game.states.PLAYING)
            secs--;
        if(secs <= 0)
        {
            Events.GameOver();
            Data.Instance.LoadLevel("GameOver");
            secs = Data.Instance.settings.totalTime;
        } else
            Invoke("Loop", 1);
    }

}
