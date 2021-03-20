using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    [HideInInspector] protected Animator anim;
    [HideInInspector] protected Character character;    

    public states state;    

    public enum states
    {
        IDLE,
        RUN,
        KICK,
        DASH,
        ACTION_DONE,
        SPECIAL_ACTION,
        GOAL,
        KICKED,
        FREEZE,
        JUMP,
        AIMING_KICK,
        DASH_WITH_BALL,
        JUEGUITO
    }
    public enum kickTypes
    {
        SOFT,
        HARD,
        BALOON,
        HEAD,
        CHILENA,
        KICK_TO_GOAL,
        DESPEJE_GOALKEEPER,
        CENTRO
    }
    Ball ball;
    private void Awake()
    {
        character = GetComponent<Character>();
    }
    public void Init(GameObject go, int teamID)
    {
        if(Game.Instance != null)
            ball = Game.Instance.ball;
        
        anim = go.GetComponent<Animator>();
        if (teamID == 1) lookTo = -1;
        else lookTo = 1;
        Vector3 scale = transform.localScale;
        scale.x *= (float)lookTo;
        transform.localScale = scale;
    }
    public void LookAtBall()
    {
        if (ball.transform.position.x > transform.position.x)
            LookTo(1);
        else
            LookTo(-1);
    }
    public virtual void Idle()
    {
        if (state == states.FREEZE || state == states.AIMING_KICK || state == states.JUEGUITO)
            return;
        if (state == states.IDLE)
        {
            PlayAnim("idle");
            return;
        } else if (state == states.FREEZE || state == states.JUMP || state == states.KICKED || state == states.SPECIAL_ACTION || state == states.KICK || state == states.DASH_WITH_BALL || state == states.DASH)
            return;
        CancelInvoke();
        this.state = states.IDLE;
        if (ball.character && ball.character.teamID == character.teamID)
            PlayAnim("idle");
        else
            Alert();


        LookAtBall();
    }
    public void Alert()
    {
        PlayAnim("alert");
    }
    int lookTo;
    public virtual void LookTo(int v )
    {
        if (character.type == Character.types.GOALKEEPER)
            return;
        if (lookTo != v)
        {
            lookTo = v;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    public bool IsRuningFast()   { return runFast;   }
    public bool runFast;
    public virtual void SuperRun()
    {
        runFast = true;
        anim.Play("runBoost", 0);
    }
    public virtual void Run()
    {
        if (state == states.FREEZE || state == states.AIMING_KICK)
            return;
        if (state == states.GOAL)
            return;
        if (state == states.IDLE)
        {
            PlayAnim("idle");
        }
        else if (state == states.JUMP || state == states.KICKED || state == states.SPECIAL_ACTION || state == states.RUN || state == states.KICK || state == states.DASH || state == states.DASH_WITH_BALL)
            return;
        this.state = states.RUN;
        PlayAnim("run");
    }
    public virtual void EnterCancha()
    {
        PlayAnim("enter");
        state = states.SPECIAL_ACTION;
    }
    public void GoalKeeperHands()
    {
        this.state = states.SPECIAL_ACTION;
        if (Random.Range(0, 10) < 5)
        {
            PlayAnim("jump");
            Invoke("ResetSpecial", 1.5f);
        }
    }
    public void Jump()
    {
        if (state == states.JUMP)
            return;
        this.state = states.JUMP;
        PlayAnim("jump");
        Invoke("ResetSpecial", 0.5f);
        character.MoveCollidersTo(new Vector3(0, 0.7f, 0));
    }
    
    public void GoalKeeperJump()
    {
        if (state == states.SPECIAL_ACTION)
            return;
        this.state = states.SPECIAL_ACTION;
        if(Random.Range(0,10)<5)
            GoalKeeperJumpType(1);
        else
            GoalKeeperJumpType(2);
    }  
    public void GoalKeeperJumpType(int id, bool resetJump = true)
    {
        if (id == 0 || lastAnimPlayed == "jump2")
        {
            PlayAnim("jump");
            if(resetJump)
                Invoke("ResetSpecial", 1.1f);
        }
        else
        {
            PlayAnim("jump2");
            if (resetJump)
                Invoke("ResetSpecial", 1.5f);
        }
    }
    public void Goal()
    {
        if (state == states.GOAL)
        {
            PlayAnim("goal");
            return;
        }            
        this.state = states.GOAL;
        PlayAnim("goal");
    }
    public void Kick(kickTypes kickType)
    {
        if (state == states.GOAL || state == states.FREEZE || state == states.KICKED || state == states.SPECIAL_ACTION || state == states.KICK)
            return;
        
        CancelInvoke();

        this.state = states.KICK;
        if (kickType == kickTypes.CHILENA)
        {
            if (character.teamID == 1)
                LookTo(1);
            else
                LookTo(-1);
            PlayAnim("chilena");
            Invoke("Reset", 0.75f);
        }
        else if (kickType == kickTypes.HEAD)
        {
            PlayAnim("head");
            Invoke("Reset", 0.5f);
        }
        else
        {
            if (kickType == kickTypes.BALOON)
                PlayAnim("kick");
            else if (kickType == kickTypes.HARD)
                PlayAnim("kick_power");
            else if (kickType == kickTypes.SOFT)
                PlayAnim("kick_soft");
            Invoke("Reset", 0.35f);
        }

    }
    public void Jueguito()
    {
        if (state == states.JUEGUITO || state == states.GOAL || state == states.FREEZE || state == states.KICKED || state == states.SPECIAL_ACTION || state == states.KICK || state == states.DASH)
            return;

        CancelInvoke();
        this.state = states.JUEGUITO;
        PlayAnim("jueguito");
    }
    public void DashWithBall()
    {
        if (state == states.GOAL || state == states.FREEZE || state == states.KICKED || state == states.SPECIAL_ACTION || state == states.KICK || state == states.DASH)
            return;

        Events.PlaySound("common", "dash", false);
        CancelInvoke();
        this.state = states.DASH_WITH_BALL;
        PlayAnim("dashWithBall");
    }
    public void Dash()
    {
        if (state == states.GOAL || state == states.FREEZE || state == states.KICKED || state == states.SPECIAL_ACTION || state == states.KICK || state == states.DASH)
            return;
        
        Events.PlaySound("common", "dash", false);
        CancelInvoke();        
        this.state = states.DASH;
        if (character.type == Character.types.GOALKEEPER)
            GoalKeeperJump();
        else
            PlayAnim("dash");
    }   
   
    public void EndDash()
    {
        if (state == states.FREEZE) return;
        Reset();
    }
    public void Reset()
    {
        character.ChangeSpeedTo(0);
        state = states.ACTION_DONE;
        Idle();
    }
    public void Cry()
    {
        CancelInvoke();
        PlayAnim("cry");
        StartFreeze(0, Data.Instance.settings.gameplay.freeze_by_loseBall);        
        Events.PlaySound("shouts", "cry", false);        
    }
    public void Kicked()
    {
        CancelInvoke();
        PlayAnim("kicked");
        StartFreeze(0, Data.Instance.settings.gameplay.freeze_by_dashBall);
        Events.PlaySound("shouts", "foul", false);        
    }   
    Coroutine freezeCoroutine;
    public void StartFreeze(float delay, float duration)
    {
        if (freezeCoroutine != null)
            StopCoroutine(freezeCoroutine);
        freezeCoroutine = StartCoroutine(Freeze(delay, duration));
    }
    public IEnumerator Freeze(float delay, float duration)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        character.SetCollidersOff(duration);
        state = states.FREEZE;
        character.Freeze();
        yield return new WaitForSeconds(duration);
        Reset();
    }
    public void Pita()
    {
        PlayAnim("start");
        Events.PlaySound("common", "pito", false);
    }
    public void Action()
    {
        if (state == states.KICKED || state == states.SPECIAL_ACTION)
            return;
        CancelInvoke();
        state = states.SPECIAL_ACTION;
        PlayAnim("action");
        Invoke("ResetSpecial", 2f);
    }
    void ResetSpecial()
    {
        if(state == states.JUMP)
            character.MoveCollidersTo(Vector3.zero);
        state = states.ACTION_DONE;
        Idle();
        
    }
    public string lastAnimPlayed;
    void PlayAnim(string animName)
    {
        if (state == states.FREEZE)
            return;
        if (lastAnimPlayed == animName)
            return;
        //if (character.isBeingControlled)
        //    print("animName " + animName);
        lastAnimPlayed = animName;
        anim.Play(animName);
    }
    public void AimingKick(bool isOn)
    {
        if (isOn)
            state = states.AIMING_KICK;
    }
}
