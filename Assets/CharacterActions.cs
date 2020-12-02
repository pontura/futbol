using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    public Animator anim;
    public Character character;
    

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
        KICKED
    }
    public enum kickTypes
    {
        SOFT,
        HARD,
        BALOON,
        HEAD,
        CHILENA,
        KICK_TO_GOAL,
        DESPEJE_GOALKEEPER
    }
    public void Init(GameObject go, int teamID)
    {
        character = GetComponent<Character>();
        anim = go.GetComponent<Animator>();
        if (teamID == 1) lookTo = -1;
        else lookTo = 1;
        Vector3 scale = transform.localScale;
        scale.x *= lookTo;
        transform.localScale = scale;
    }
    public void LookAtBall()
    {
        if (Game.Instance.ball.transform.position.x > transform.position.x)
            LookTo(1);
        else
            LookTo(-1);
    }
    public virtual void Idle()
    {
        if (state == states.KICKED || state == states.SPECIAL_ACTION || state == states.IDLE || state == states.KICK)
            return;
        this.state = states.IDLE;
        anim.Play("idle");
        LookAtBall();
    }
    int lookTo;
    public virtual void LookTo(int v )
    {
        if (character.isGoldKeeper)
            return;
        if (lookTo != v)
        {
            lookTo = v;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    void LookToAttack()
    {

    }
    public virtual void Run()
    {
        if (state == states.KICKED || state == states.SPECIAL_ACTION || state == states.RUN || state == states.KICK || state == states.DASH)
            return;
        this.state = states.RUN;
        anim.Play("run");
    }
    public virtual void EnterCancha()
    {
        anim.Play("enter");
    }
    public void GoalKeeperHands()
    {
        this.state = states.SPECIAL_ACTION;
        if (Random.Range(0, 10) < 5)
        {
            anim.Play("jump");
            Invoke("ResetSpecial", 1.5f);
        }
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
        if (id == 0)
        {
            anim.Play("jump");
            if(resetJump)
                Invoke("ResetSpecial", 1.1f);
        }
        else
        {
            anim.Play("jump2");
            if (resetJump)
                Invoke("ResetSpecial", 1.5f);
        }
    }
    public void Goal()
    {
        if (state == states.KICKED || state == states.GOAL)
            return;
        this.state = states.GOAL;
        anim.Play("goal");
    }
    public void Kick(kickTypes kickType)
    {
        if (state == states.KICKED || state == states.SPECIAL_ACTION || state == states.KICK)
            return;
        
        CancelInvoke();

        this.state = states.KICK;
        if (kickType == kickTypes.CHILENA)
        {
            if (character.teamID == 1)
                LookTo(1);
            else
                LookTo(-1);
            anim.Play("chilena");
            Invoke("Reset", 0.75f);
        }
        else if (kickType == kickTypes.HEAD)
        {
            anim.Play("head");
            Invoke("Reset", 0.5f);
        }
        else
        {
            if (kickType == kickTypes.BALOON)
                anim.Play("kick");
            else if (kickType == kickTypes.HARD)
                anim.Play("kick_power");
            else if (kickType == kickTypes.SOFT)
                anim.Play("kick_soft");
            Invoke("Reset", 0.35f);
        }

    }
    public void Dash()
    {
        if (state == states.KICKED || state == states.SPECIAL_ACTION || state == states.KICK || state == states.DASH)
            return;
        Events.PlaySound("common", "dash", false);
        CancelInvoke();
        this.state = states.DASH;
        anim.Play("dash");
        character.ChangeSpeedTo(Data.Instance.settings.gameplay.speedDash);

        Invoke("Reset", 0.25f);
    }
    public void Reset()
    {
        CancelInvoke();
        character.ChangeSpeedTo(0);
        state = states.ACTION_DONE;
    }
    public void Kicked()
    {
        state = states.KICKED;
        anim.Play("kicked");
        Invoke("ResetSpecial", 0.5f);
    }
    public void Pita()
    {
        anim.Play("start");

        Events.PlaySound("common", "pito", false);
    }
    public void Action()
    {
        if (state == states.KICKED || state == states.SPECIAL_ACTION)
            return;
        CancelInvoke();
        state = states.SPECIAL_ACTION;
        anim.Play("action");
        Invoke("ResetSpecial", 2f);
    }
    void ResetSpecial()
    {
        state = states.ACTION_DONE;
        Idle();
    }
}
