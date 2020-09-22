using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    public int id; //for player Input;
    public int characterID; //for data;
    Collider[] colliders;
    public float speed;
    public Transform characterContainer;

    [HideInInspector] public Ball ball;
    [HideInInspector] public CharactersManager charactersManager;
    public int teamID;
    [HideInInspector] public TextsData.CharactersData data;    
    [HideInInspector] public CharacterActions actions;
    [HideInInspector] public CharacterSignal characterSignal;
    [HideInInspector] public BallCatcher ballCatcher;
    public bool isBeingControlled;
    [HideInInspector] public AI ai;
    [HideInInspector] public bool isGoldKeeper;

    void Awake()
    {
        if (GetComponent<GoalKeeper>())
            isGoldKeeper = true;
        actions = GetComponent<CharacterActions>();
        ballCatcher = GetComponent<BallCatcher>();
        ai = GetComponent<AI>();
    }
    public virtual void Start()
    {
        colliders = GetComponents<Collider>();
        Loop();
    }
    public void Init(int _temaID, CharactersManager charactersManager, GameObject asset_to_instantiate)
    {
        if (isGoldKeeper)
            speed = Data.Instance.settings.goalKeeperSpeed;
        else
            speed = Data.Instance.settings.speed;
        this.characterID = int.Parse (asset_to_instantiate.name); //con el nombre sacamos el id:
        data = Data.Instance.textsData.GetCharactersData(characterID, isGoldKeeper);
        this.charactersManager = charactersManager;
        this.teamID = _temaID;
        GameObject asset = Instantiate(asset_to_instantiate);
        asset.transform.SetParent(characterContainer);
        asset.transform.localEulerAngles = asset.transform.localPosition = Vector3.zero;
        asset.transform.localScale = Vector3.one;
        actions.Init(asset, teamID);
    }

    void Loop()
    {
        if (Game.Instance.state == Game.states.PLAYING)
        {
            Vector3 dest = Game.Instance.cameraInGame.transform.localEulerAngles;
            if (transform.localScale.x < 0)
            {
                dest.y *= -1;
                dest.z *= -1;
            }
            characterContainer.localEulerAngles = dest;
        }
        Invoke("Loop", 0.1f);
    }
    public void OnCatch(Ball _ball)
    {
        actions.Reset();
        speed = Data.Instance.settings.speedWithBall;
        this.ball = _ball;
        ballCatcher.Catch(ball);
        charactersManager.CharacterCatchBall(this);
        Events.PlaySound("common", "ballSnap", false); 
        Events.CharacterCatchBall(this);
    }
    public virtual void SetPosition(int _x, int _y)
    {
        MoveTo(_x, _y);            
    }
    public void Kick(CharacterActions.kickTypes kickType, float forceForce = 0)
    {
        actions.Kick(kickType);
        if (ball != null && ball.GetCharacter() == this)
        {
            ball.Kick(kickType, forceForce);
            ballCatcher.LoseBall();
        }
    }
    public void Dash()
    {
        actions.Dash();
    }
    public void ChangeSpeedTo(float value)
    {
        speed = Data.Instance.settings.speed + value;
    }
    public virtual void MoveTo(int _x, int _y)
    {
        if (_x == 0 && _y == 0)
            actions.Idle();
        else
        {
            if(_x != 0)
                actions.LookTo(_x);
            actions.Run();
        }
        if(ballCatcher != null)
            ballCatcher.SetRotation(_x, _y);
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
    public void OnGoal(bool win)
    {
        actions.Idle();
        ai.ResetAll();
    }
    public void SetCollidersOff()
    {
        foreach (Collider c in colliders)
            c.enabled = false;
        Invoke("ResetColliders", 0.2f); 
    }
    void ResetColliders()
    {
        foreach (Collider c in colliders)
            c.enabled = true;
    }
}
