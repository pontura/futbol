﻿using System.Collections;
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
        GOAL
    }
    public enum kickTypes
    {
        SOFT,
        HARD,
        BALOON,
        HEAD,
        CHILENA,
        KICK_TO_GOAL
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
        if (state == states.SPECIAL_ACTION || state == states.IDLE || state == states.KICK)
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
        if (state == states.SPECIAL_ACTION || state == states.RUN || state == states.KICK || state == states.DASH)
            return;
        this.state = states.RUN;
        anim.Play("run");
    }
    public void GoalKeeperJump()
    {
        if (state == states.SPECIAL_ACTION)
            return;
        this.state = states.SPECIAL_ACTION;
        if(Random.Range(0,10)<5)
        {
            anim.Play("jump2");
            Invoke("ResetSpecial", 1.5f);
        }
        else
        {
            anim.Play("jump");
            Invoke("ResetSpecial", 1.1f);
        }
        
    }
    public void Goal()
    {
        if (state == states.GOAL)
            return;
        this.state = states.GOAL;
        anim.Play("goal");
    }
    public void Kick(kickTypes kickType)
    {       
        if (state == states.KICK)
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
        if (state == states.KICK || state == states.DASH)
            return;
        CancelInvoke();
        this.state = states.DASH;
        anim.Play("dash");
        character.ChangeSpeedTo(Data.Instance.settings.speedDash);

        Invoke("Reset", 0.25f);
    }
    public void Reset()
    {
        CancelInvoke();
        character.ChangeSpeedTo(0);
        state = states.ACTION_DONE;
    }
    public void Pita()
    {
        anim.Play("start");
    }
    public void Action()
    {       
        if (state == states.SPECIAL_ACTION)
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
