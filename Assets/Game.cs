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
        GOAL
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
    }
    private void Start()
    {
        Events.OnGameStatusChanged += OnGameStatusChanged;
        cameraInGame.SetTargetTo(ball.transform);
        Events.PlaySound("crowd", "crowd_quiet", true);
        charactersManager.Init(1);
        StartCoroutine( OnWaitToStart() );        
    }
    private void OnDestroy()
    {
        Events.OnGameStatusChanged -= OnGameStatusChanged;
    }
    public IEnumerator OnWaitToStart()
    {
        state = states.WAITING;
        yield return new WaitForSeconds(0.6f);
        VoicesManager.Instance.SayResults();
        yield return new WaitForSeconds(2);
        state = states.PLAYING;
        Events.OnGameStatusChanged(Game.states.PLAYING);
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
            Events.SetDialogue(ch, Data.Instance.textsData.GetRandomDialogue("goal", ch.characterID, ch.isGoldKeeper));


        ball.KickIfOnGoal();
        state = states.GOAL;
        Events.OnGameStatusChanged(state);        
        
        StartCoroutine(GetComponent<GoalMoment>().Init(teamID, character));
    }
    void OnGameStatusChanged(states state)
    {
        this.state = state;
    }
}