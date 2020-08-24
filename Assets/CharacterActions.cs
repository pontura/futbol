using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    Animator anim;
    public Character character;
    

    public states state;
    

    public enum states
    {
        IDLE,
        RUN,
        KICK,
        DASH,
        ACTION_DONE,
        SPECIAL_ACTION
    }
    public enum kickTypes
    {
        SOFT,
        HARD,
        BALOON,
        HEAD,
        CHILENA
    }
    private void Start()
    {
       
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
    public void Idle()
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
    public void Run()
    {
        if (state == states.SPECIAL_ACTION || state == states.RUN || state == states.KICK || state == states.DASH)
            return;
        this.state = states.RUN;
        anim.Play("run");
    }
    public void Kick(kickTypes kickType)
    {       
        if (state == states.KICK)
            return;
        
        CancelInvoke();
        print("Kick " + kickType);

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
            anim.Play("kick");
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
