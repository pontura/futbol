﻿using System.Collections;
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
        cameraInGame.SetTargetTo(ball.transform);
        Events.PlaySound("crowd", "crowd_quiet");
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
            Events.SetDialogue(ch, Data.Instance.textsData.GetRandomDialogue("goal", ch.characterID));


        ball.KickIfOnGoal();
        state = states.GOAL;
        Events.OnGameStatusChanged(state);
        
        
        StartCoroutine(GetComponent<GoalMoment>().Init(teamID, character));
    }
   
}