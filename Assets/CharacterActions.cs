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
        JUMP
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
        if (state == states.IDLE)
        {
            PlayAnim("idle");
            return;
        } else if (state == states.JUMP || state == states.KICKED || state == states.SPECIAL_ACTION || state == states.KICK || state == states.DASH)
            return;
        StopAllCoroutines();
        CancelInvoke();
        this.state = states.IDLE;
        PlayAnim("idle");
        LookAtBall();
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
        PlayAnim("runBoost");
    }
    public virtual void Run()
    {
        runFast = false;
        if (state == states.GOAL)
            return;
        if (state == states.IDLE)
        {
        }
        else if (state == states.JUMP || state == states.FREEZE || state == states.KICKED || state == states.SPECIAL_ACTION || state == states.RUN || state == states.KICK || state == states.DASH)
            return;
        this.state = states.RUN;
        PlayAnim("run");
    }
    public virtual void EnterCancha()
    {
        PlayAnim("enter");
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
        character.MoveCollidersTo(new Vector3(0, 1, 0));
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
    public void Dash()
    {
        if (state == states.GOAL || state == states.FREEZE || state == states.KICKED || state == states.SPECIAL_ACTION || state == states.KICK || state == states.DASH)
            return;
        Events.PlaySound("common", "dash", false);
        CancelInvoke();        
        this.state = states.DASH;
        PlayAnim("dash");
    }   
    public IEnumerator Freeze(float delay, float duration)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);        
        state = states.FREEZE;
        character.Freeze();
        if(duration>0)
            yield return new WaitForSeconds(duration);
        EndDash();
    }
    public void EndDash()
    {
        Reset();
    }
    public void Reset()
    {
        character.ChangeSpeedTo(0);
        state = states.ACTION_DONE;
        Idle();
    }
    public void Kicked()
    {
        CancelInvoke();
        StopAllCoroutines();
        StartCoroutine(Freeze(0, Data.Instance.settings.gameplay.freeze_by_dashBall));
        PlayAnim("kicked");
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
        if (lastAnimPlayed == animName)
            return;
        //if (character.isBeingControlled)
        //    print("animName " + animName);
        lastAnimPlayed = animName;
        anim.Play(animName);
    }
}
