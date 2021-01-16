using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector] public CharactersData.CharacterData dataSources;
    public int control_id; //for player Input;
    Collider[] colliders;
    public float speed;
    public Transform characterContainer;
    public Settings.GamePlay stats;
    public types type;
    public enum types
    {
        DEFENSOR,
        CENTRAL,
        DELANTERO,
        GOALKEEPER,
        REFERI
    }
    public fieldPositions fieldPosition;
    public enum fieldPositions
    {
       UP,
       CENTER,
       DOWN
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
    Character oponent;
    Vector2 limits_y;
    public Vector2 limits_x;

    void Awake()
    {
        actions = GetComponent<CharacterActions>();
        ballCatcher = GetComponent<BallCatcher>();
        ai = GetComponent<AI>();
    }
    public virtual void Start()
    {
        colliders = GetComponents<Collider>();
        Loop();
    }
    public Character Oponent
    {
        get {
            if (oponent == null)
                oponent = charactersManager.GetOponentFor(this);
            return oponent;
        }
    }
    public void Init(int _temaID, CharactersManager charactersManager, GameObject asset_to_instantiate)
    {
        stats = Data.Instance.settings.GetStats(type, _temaID);
        ballCatcher.Show(false);
        scaleFactor = Data.Instance.settings.scaleFactor;
        this.charactersManager = charactersManager;
        this.teamID = _temaID;

        int characterID = int.Parse(asset_to_instantiate.name); //con el nombre sacamos el id:
        data = Data.Instance.textsData.GetCharactersData(characterID, type == Character.types.GOALKEEPER);
        if (type == Character.types.GOALKEEPER)
        {
            dataSources = CharactersData.Instance.all_goalkeepers[data.id - 1];
            speed = stats.goalKeeperSpeed;
        }   else
        {
            dataSources = CharactersData.Instance.all[data.id - 1];
            speed = stats.speed;
        }
        
        GameObject asset = Instantiate(asset_to_instantiate);
        asset.transform.SetParent(characterContainer);
        asset.transform.localEulerAngles = asset.transform.localPosition = Vector3.zero;
        asset.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        actions.Init(asset, teamID);
        ai.Init();
        SetLimits();

        if (type == types.GOALKEEPER)
        {
            limits_y /= 1.8f;

            if (teamID == 1)
                limits_x = new Vector2(limits_x.y- 5, limits_x.y);
            else
                limits_x = new Vector2(limits_x.x, limits_x.x + 5);
        }
        
    }
    public void SetLimits()
    {
        float _limits_x = Data.Instance.stadiumData.active.size_x / 2;
        float _limits_y = Data.Instance.stadiumData.active.size_y / 2;

        limits_y = new Vector2(_limits_y, -_limits_y);
        limits_x = new Vector2(-_limits_x, _limits_x);
    }
    public Vector2 SetPositionInsideLimits(Vector2 newPos)
    {
        Vector2 pos = newPos;
        if (newPos.x < limits_x.x)
            pos.x = limits_x.x;
        else if(newPos.x > limits_x.y)
            pos.x = limits_x.y;
        if (newPos.y > limits_y.x)
            pos.y = limits_y.x;
        else if (newPos.y < limits_y.y)
            pos.y = limits_y.y;
        return pos;
    }
    public bool IsPositionOutsideLimitsZ(float pos)
    {
        if (pos < limits_y.y || pos > limits_y.y)
            return true;
        return false;
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
        Character other = _ball.character;
        if (other != null)
        {
            if (actions.state == CharacterActions.states.DASH)
            {
                other.actions.Kicked();
                Vector3 pos = other.transform.position;
                if (
                    (pos.x > limits_x.y*0.65f && teamID == 1) || (pos.x < limits_x.x * 0.65f && teamID == 2)
                    &&
                     (pos.z < 7.25f && pos.z > -7.25f)
                    &&
                    (other.teamID != teamID)
                    )
                {
                    Invoke("PenaltyDelayed", 0.25f);
                    return;
                }
            }
            else
                other.actions.Cry();
        }
       // actions.Reset();
        speed = stats.speedWithBall;
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
    public void StartKicking()
    {
        actions.AimingKick(true);
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
        charactersManager.Swap(control_id);
    }
    Coroutine dashCoroutine;
    public void Dash()
    {
        if (actions.state == CharacterActions.states.DASH || actions.state == CharacterActions.states.FREEZE)
            return;
        if(dashCoroutine != null)
            StopCoroutine(dashCoroutine);
        dashCoroutine = StartCoroutine(DashC());
        actions.Dash();
    }
    public void Jueguito()
    {
        if (actions.state == CharacterActions.states.JUEGUITO)
            return;
        StopAllCoroutines();
        actions.Jueguito();
        ballCatcher.Jueguito();
    }
    float lastTimeDashWithBall = 0;
    public void Dash_with_ball()
    {
        if (lastTimeDashWithBall != 0 && lastTimeDashWithBall + 1 > Time.time)
            return;
        if (actions.state == CharacterActions.states.DASH || actions.state == CharacterActions.states.FREEZE)
            return;
        if (dashCoroutine != null)
            StopCoroutine(dashCoroutine);
        lastTimeDashWithBall = Time.time;
        dashCoroutine = StartCoroutine(DashC());
        actions.DashWithBall();
        ballCatcher.Jump();
    }
    IEnumerator DashC()
    {        
        ChangeSpeedTo(stats.speedDash);
        yield return new WaitForSeconds(0.25f);
        if (ai.ball.character != this)
            actions.StartFreeze(0, stats.freeze_dash);
        else
            actions.EndDash();
        ChangeSpeedTo(0);
        yield return null;
    }
    public void ChangeSpeedTo(float value)
    {
        if(Game.Instance.ball.character == this)
            speed = stats.speedWithBall + value;
        else
            speed = stats.speed + value;
    }
    public void Freeze()
    {
        speed = 0;
    }
    Vector2 direction;
    public virtual void MoveTo(float _x, float _y)
    {

        if (actions.state == CharacterActions.states.GOAL || actions.state == CharacterActions.states.FREEZE)
            return;
        else if (actions.state == CharacterActions.states.JUEGUITO)
        {
            if (_x == 0)
                return;
            else ballCatcher.Reset();
        }
       // if (actions.state == CharacterActions.states.DASH_WITH_BALL)
        //    if (teamID == 1) _x = -1; else _x = 1; // corre si o si mientras hace un dash


        if (_x == 0 && _y == 0)
            actions.Idle();
        else
        {            
            if (_x != 0 && actions.state != CharacterActions.states.AIMING_KICK)
            {
                if(_x<1) actions.LookTo(-1);
                else actions.LookTo(1);
            }                
            actions.Run();
        }

        if (transform.position.z > limits_y.x ) _y = -1;
        else if (transform.position.z < limits_y.y) _y = 1;

        Vector3 aimVector = (Vector3.right * _x * speed * Time.deltaTime) + (Vector3.forward * _y * speed * Time.deltaTime);
        if (actions.state == CharacterActions.states.AIMING_KICK)
        {
            _x = direction.x;
            _y = direction.y;
        }
        else
        {
            direction = new Vector2(_x, _y);
        }
        Vector3 forwardVector = (Vector3.right * _x * speed * Time.deltaTime) + (Vector3.forward * _y * speed * Time.deltaTime);
        
        if (actions.state == CharacterActions.states.AIMING_KICK)
            aimVector = aimVector / 10;

        if (ballCatcher != null)
            ballCatcher.RotateTo(aimVector);

        Vector3 pos = transform.position;
        if (transform.position.x > limits_x.y && _x>0)
        {
            pos.x = limits_x.y;
            transform.position = pos;
        }
        else if (transform.position.x < limits_x.x && _x < 0)
        {
            pos.x = limits_x.x;
            transform.position = pos;
        }
        transform.Translate(forwardVector);

      
    }
    public void SetSignal(CharacterSignal signal)
    {
        characterSignal = signal;

        if(signal == null)
        {
            Debug.LogError("no hay signal");
            return;
        }           
        if (actions == null)
        {
            Debug.LogError("no hay signal");
            return;
        }
        

        signal.transform.SetParent(actions.transform);
        signal.transform.localScale = Vector3.one;
        signal.transform.localPosition = Vector3.zero;        
    }
    public void SetControlled(bool _isBeingControlled)
    {
        ballCatcher.Show(_isBeingControlled);
        this.isBeingControlled = _isBeingControlled;
    }
    public void OnGoal(bool win)
    {
        if(Game.Instance.state != Game.states.PENALTY)
            actions.Idle();

        if(ai.enabled)
            ai.ResetAll();
    }
    public void SetCollidersOff(float delay)
    {
        foreach (Collider c in colliders)
            c.enabled = false;
        Invoke("ResetColliders", delay); 
    }
    Vector3 collidersOriginalPos = Vector3.zero;
    public void MoveCollidersTo(Vector3 pos)
    {
        if (colliders.Length == 0) return;
        colliders[0].GetComponent<CapsuleCollider>().center = collidersOriginalPos + pos;
        Invoke("ResetColliders", 0.5f); // resetea posicion de c desepues de saltar:
    }
    public void Reset()
    {
        ResetColliders();
    }
    void ResetColliders()
    {
        foreach (Collider c in colliders)
            c.enabled = true;
        colliders[0].GetComponent<CapsuleCollider>().center = collidersOriginalPos;
    }
    public void Jump()
    {
        if(type != types.GOALKEEPER)
            actions.Jump();
    }
    Coroutine runCoroutine;
    public void SuperRun()
    {
        if (actions.state == CharacterActions.states.DASH)
            return;
        if(runCoroutine != null)
            StopCoroutine(runCoroutine);
        runCoroutine = StartCoroutine(RunSpeedDesacelerate());
    }
    IEnumerator RunSpeedDesacelerate()
    {     
        actions.SuperRun();
        float minSpeed;

        if(Game.Instance.ball.character == this)
        {
            speed = stats.speedRunWithBall;
            minSpeed = stats.speedWithBall;
        }
        else
        {
            speed = stats.speedRun;
            minSpeed = stats.speed;
        }   

        while (speed > minSpeed)
        {
            speed -= stats.speedRunFade * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if(Game.Instance.state == Game.states.PLAYING)
            actions.Run();
        speed = minSpeed;
        yield break;
    }
}
