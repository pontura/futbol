using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    static Game mInstance = null;
    public CharactersManager charactersManager;
    public CameraInGame cameraInGame;
    public Ball ball;

    public states state;
    public enum states
    {
        WAITING,
        PLAYING,
        GOAL,
        PENALTY,
        GAMEOVER
    }

    public static Game Instance
    {
        get
        {
            return mInstance;
        }
    }
    void Awake()
    {
        if (!mInstance)
            mInstance = this;
        Events.OnGameStatusChanged += OnGameStatusChanged;
        Events.OnPenalty += OnPenalty;
    }
    private void Start()
    {
        Time.timeScale = Data.Instance.settings.timeScale;
        StartCoroutine(WaitToStart());
    }
    IEnumerator WaitToStart()
    {
        while (!Data.Instance.settings.loaded)
            yield return new WaitForEndOfFrame();
        OnInit();
    }
    private void OnInit()
    {
        DialoguesManager d = GetComponent<DialoguesManager>();
        if(d != null)  d.Init();

        cameraInGame.SetTargetTo(ball.transform);
        cameraInGame.Restart();
        Events.PlaySound("crowd", "crowd_quiet", true);        
        Events.PlaySound("common", "ballEnter", false);
        if (state == states.PENALTY)
            charactersManager.InitPenalty();
        else
        {
            charactersManager.Init();
            StartCoroutine(OnWaitToStart());
        }
        Events.GameInit();
    }
    
    private void OnDestroy()
    {
        Events.OnGameStatusChanged -= OnGameStatusChanged;
        Events.OnPenalty -= OnPenalty;
    }
    public IEnumerator OnWaitToStart()
    {
        cameraInGame.Restart();
        cameraInGame.SetTargetTo(ball.transform);        
        state = states.WAITING;
        yield return new WaitForSeconds(0.6f);
        VoicesManager.Instance.SayResults();
        Events.PlaySound("crowd", "crowd_quiet", true);
        yield return new WaitForSeconds(2);
        state = states.PLAYING;
        Events.OnGameStatusChanged(Game.states.PLAYING);
        yield return new WaitForSeconds(0.6f);        
        cameraInGame.Reset();
        
    }
    public void Goal(int teamID, Character character)
    {
        List<Character> winner;
        List<Character> loser;
        if (teamID == 1)
        {
            winner = charactersManager.team1;
            loser = charactersManager.team2;
        }
        else
        {
            winner = charactersManager.team2;
            loser = charactersManager.team1;
        }
        
        foreach (Character ch in winner)
            Events.SetDialogue(ch, Data.Instance.textsData.GetRandomDialogue("goal", ch.data.id, ch.type == Character.types.GOALKEEPER));


        ball.KickIfOnGoal();
        state = states.GOAL;
        Events.OnGameStatusChanged(state);        
        
        GetComponent<GoalMoment>().Init(teamID, character);
    }
    void OnGameStatusChanged(states state)
    {
        this.state = state;            
    }
    void OnPenalty(Character ch)
    {
        Data.Instance.matchData.penaltyGoalKeeperTeamID = ch.teamID;
        state = states.PENALTY;
        Time.timeScale = 0;
    }
}