﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    CharactersManager charactersManager;
    Ball ball;
    [HideInInspector]
    public int teamID;
    public int id;
    float speed;
    float original_speed = 8;
    [SerializeField] private Transform characterContainer;
    [HideInInspector] public CharacterActions actions;
    [HideInInspector] public CharacterSignal characterSignal;
    [HideInInspector] public BallCatcher ballCathcer;
    [HideInInspector] public bool isBeingControlled;
    [HideInInspector] public AI ai;

    private void Awake()
    {
        speed = original_speed;
        actions = GetComponent<CharacterActions>();
        ballCathcer = GetComponent<BallCatcher>();
        ai = GetComponent<AI>();
    }
    public void Init(int _temaID, CharactersManager charactersManager)
    {
        this.charactersManager = charactersManager;
        this.teamID = _temaID;
        GameObject go = Instantiate(Resources.Load<GameObject>("players/" + teamID) as GameObject);
        go.transform.SetParent(characterContainer);
        go.transform.localEulerAngles = go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        actions.Init(go, teamID);
    }
    public void OnCatch(Ball _ball)
    {
        this.ball = _ball;
        ballCathcer.Catch(ball);
        charactersManager.CharacterCatchBall(this);
        Events.CharacterCatchBall(this);
    }
    public void OnBallTriggerEnter(Ball _ball)
    {
        ball = _ball;
    }
    public void OnBallTriggerExit(Ball _ball)
    {
        ball = null;
    }
    public void SetPosition(int _x, int _y)
    {
        MoveTo(_x, _y);            
    }
    public void Kick(CharacterActions.kickTypes kickType)
    {
        actions.Kick(kickType);

        if (ball != null && ball.GetCharacter() == this)
        {
            ball.Kick(kickType);
            ballCathcer.LoseBall();
        }
    }
    public void Dash()
    {
        actions.Dash();
    }
    public void ChangeSpeedTo(float value)
    {
        speed = original_speed + value;
    }
    void MoveTo(int _x, int _y)
    {
        if (_x == 0 && _y == 0)
            actions.Idle();
        else
        {
            if(_x != 0)
                actions.LookTo(_x);
            actions.Run();
        }

        transform.Translate(Vector3.right * _x * speed*Time.deltaTime + Vector3.forward * _y * speed * Time.deltaTime);
    }
    public void SetSignal(CharacterSignal signal)
    {
        characterSignal = signal;
        signal.transform.SetParent(actions.transform);
        signal.transform.localScale = Vector3.one;
        signal.transform.localPosition = Vector3.zero;        
    }
    public void SetControlled(bool _isBeingControlled)
    {
        this.isBeingControlled = _isBeingControlled;
    }
}
