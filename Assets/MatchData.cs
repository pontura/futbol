using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchData : MonoBehaviour
{
    public int[] players;
    public int secs;
    public Vector2 score;
    public int penaltyGoalKeeperTeamID;
    public int totalPlayers;
    public int lastGoalBy;
    public int totalCharacters;
    public int[] charactersPositions;

    public bool team1Controlled;
    public bool team2Controlled;

    private void Awake()
    {
        AddPlayer(1, 1);
    }
    void Start()
    {        
        secs = Data.Instance.settings.totalTime;
        Events.GameInit += GameInit;        
    }
    public void AddPlayer(int id, int teamID)
    {
        players[id - 1] = teamID;
        totalPlayers++;
        if (teamID == 1) team1Controlled = true;
        if (teamID == 2) team2Controlled = true;
    }
    public bool IsTeamBeingControlled(int teamID)
    {
        if (teamID == 1 && team1Controlled) return true;
        if (teamID == 2 && team2Controlled) return true;
        else return false;
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
    public void SetCharactersPositions(int team1, int team2)
    {
        charactersPositions[0] = team1;
        charactersPositions[1] = team2;
    }

}
