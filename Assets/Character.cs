using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Ball ball;
    float kickForce = 1000;
    [HideInInspector]
    public int teamID;
    public int id;
    public float speed = 5;
    [SerializeField] private Transform characterContainer;
    [HideInInspector] public CharacterActions actions;
    [HideInInspector] public CharacterSignal characterSignal;
    [HideInInspector] public BallCatcher ballCathcer;
    private void Awake()
    {
        actions = GetComponent<CharacterActions>();
        ballCathcer = GetComponent<BallCatcher>();
    }
    public void Init(int _temaID)
    {
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
    public void Kick()
    {
        actions.Kick();

        if (ball != null && ball.GetCharacter() == this)
        {
            ball.Kick(kickForce);
            ballCathcer.LoseBall();
        }
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
}
