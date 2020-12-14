using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector] public CharactersData.CharacterData dataSources;
    public int id; //for player Input;
    Collider[] colliders;
    public float speed;
    public Transform characterContainer;
    public types type;
    public enum types
    {
        DEFENSOR_UP,
        DEFENSOR_DOWN,
        CENTRAL,
        DELANTERO_UP,
        DELANTERO_DOWN,
        GOALKEEPER
    }
    [HideInInspector] public CharactersManager charactersManager;
    public int teamID;
    public TextsData.CharacterData data;    
    [HideInInspector] public CharacterActions actions;
    [HideInInspector] public CharacterSignal characterSignal;
    [HideInInspector] public BallCatcher ballCatcher;
    public bool isBeingControlled;
    [HideInInspector] public AI ai;
    [HideInInspector] public float scaleFactor;

    void Awake()
    {
        if (GetComponent<GoalKeeper>())
            type = types.GOALKEEPER;
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
        scaleFactor = Data.Instance.settings.gameplay.scaleFactor;
        this.charactersManager = charactersManager;
        this.teamID = _temaID;

        int characterID = int.Parse(asset_to_instantiate.name); //con el nombre sacamos el id:
        data = Data.Instance.textsData.GetCharactersData(characterID, type == Character.types.GOALKEEPER);
        if (type == Character.types.GOALKEEPER)
        {
            dataSources = CharactersData.Instance.all_goalkeepers[data.id - 1];
            speed = Data.Instance.settings.gameplay.goalKeeperSpeed;
        }   else
        {
            dataSources = CharactersData.Instance.all[data.id - 1];
            speed = Data.Instance.settings.gameplay.speed;
        }
        
        GameObject asset = Instantiate(asset_to_instantiate);
        asset.transform.SetParent(characterContainer);
        asset.transform.localEulerAngles = asset.transform.localPosition = Vector3.zero;
        asset.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        actions.Init(asset, teamID);
        
    }

    void Loop()
    {
        //if (Game.Instance.state == Game.states.PLAYING)
        //{
            Vector3 dest = Game.Instance.cameraInGame.transform.localEulerAngles;
            if (transform.localScale.x < 0)
            {
                dest.y *= -1;
                dest.z *= -1;
            }
            characterContainer.localEulerAngles = dest;
       // }
        Invoke("Loop", 0.1f);
    }
    public void OnCatch(Ball _ball)
    {
        if (_ball.character != null)
        {
            if (actions.state == CharacterActions.states.DASH)
            {
                _ball.character.actions.Kicked();
                if ((transform.localPosition.x > 13 && teamID == 1) || (transform.localPosition.x < -13 && teamID == 2))
                {
                    Invoke("PenaltyDelayed", 0.15f);
                    return;
                }
            }
            else
            {
                _ball.character.actions.StartCoroutine( _ball.character.actions.Freeze(0, Data.Instance.settings.gameplay.freeze_by_loseBall) );
            }
        }
       // actions.Reset();
        speed = Data.Instance.settings.gameplay.speedWithBall;
        ballCatcher.Catch(Game.Instance.ball);
        charactersManager.CharacterCatchBall(this);
        Events.PlaySound("common", "ballSnap", false); 
        Events.CharacterCatchBall(this);
    }
    void PenaltyDelayed()
    {
        Events.OnPenalty(this);
    }
    public virtual void SetPosition(float _x, float _y)
    {
        MoveTo(_x, _y);            
    }

    public void Kick(CharacterActions.kickTypes kickType, float forceForce = 0)
    {
        actions.Kick(kickType);
        if (Game.Instance.ball.GetCharacter() == this)
        {
            if (Game.Instance.ball.character.type == Character.types.GOALKEEPER)
                Invoke("AutomaticChangePlayer", 0.1f);
            Game.Instance.ball.Kick(kickType, forceForce);
            ballCatcher.LoseBall();            
        }
    }
    void AutomaticChangePlayer()
    {
        charactersManager.Swap(id);
    }
    public void Dash()
    {
        actions.Dash();
    }
    public void ChangeSpeedTo(float value)
    {
        if(Game.Instance.ball.character == this)
            speed = Data.Instance.settings.gameplay.speedWithBall + value;
        else
            speed = Data.Instance.settings.gameplay.speed + value;
    }
    public void Freeze()
    {
        speed = 0;
    }
    public virtual void MoveTo(float _x, float _y)
    {
        if (actions.state == CharacterActions.states.GOAL || actions.state == CharacterActions.states.FREEZE)
            return;
        if (_x == 0 && _y == 0)
            actions.Idle();
        else
        {
            if(_x != 0)
            {
                if(_x<1) actions.LookTo(-1);
                else actions.LookTo(1);
            }
                
            actions.Run();
        }
        Vector3 forwardVector = Vector3.right * _x * speed * Time.deltaTime + Vector3.forward * _y * speed * Time.deltaTime;
        if (ballCatcher != null)
            ballCatcher.RotateTo(forwardVector);
           // ballCatcher.SetRotation((int)_x, (int)_y);
        transform.Translate(forwardVector);
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
        if(Game.Instance.state != Game.states.PENALTY)
            actions.Idle();

        if(ai.enabled)
            ai.ResetAll();
    }
    public void SetCollidersOff()
    {
        foreach (Collider c in colliders)
            c.enabled = false;
        Invoke("ResetColliders", 0.3f); 
    }
    Vector3 collidersOriginalPos = Vector3.zero;
    public void MoveCollidersTo(Vector3 pos)
    {
        if (colliders.Length == 0) return;
        colliders[0].GetComponent<CapsuleCollider>().center = collidersOriginalPos + pos;
    }
    void ResetColliders()
    {
        foreach (Collider c in colliders)
            c.enabled = true;
    }
    public void Jump()
    {
        if(type != types.GOALKEEPER)
            actions.Jump();
    }
    public void SuperRun()
    {
        if (actions.IsRuningFast())
            return;
        StopAllCoroutines();
        StartCoroutine(RunSpeedDesacelerate());
    }
    IEnumerator RunSpeedDesacelerate()
    {     
        actions.SuperRun();
        float minSpeed;

        if(Game.Instance.ball.character == this)
        {
            speed = Data.Instance.settings.gameplay.speedRunWithBall;
            minSpeed = Data.Instance.settings.gameplay.speedWithBall;
        }
        else
        {
            speed = Data.Instance.settings.gameplay.speedRun;
            minSpeed = Data.Instance.settings.gameplay.speed;
        }   

        while (speed > minSpeed)
        {
            speed -= Data.Instance.settings.gameplay.speedRunFade * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        actions.Run();
        speed = minSpeed;
    }
}
