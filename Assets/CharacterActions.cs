using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    Animator anim;
    public Character character;

    public states state;
    Collider[] colliders;

    public enum states
    {
        IDLE,
        RUN,
        KICK,
        DASH,
        ACTION_DONE
    }
    public enum kickTypes
    {
        SOFT,
        HARD,
        BALOON
    }
    private void Start()
    {
        colliders = GetComponents<Collider>();
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
    public void Idle()
    {
        if (state == states.IDLE || state == states.KICK)
            return;
        this.state = states.IDLE;
       anim.Play("idle");
    }
    int lookTo;
    public virtual void LookTo(int v )
    {
        if (lookTo != v)
        {
            lookTo = v;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    public void Run()
    {
        if (state == states.RUN || state == states.KICK || state == states.DASH)
            return;
        this.state = states.RUN;
        anim.Play("run");
    }
    public void Kick(kickTypes kickType)
    {
        CancelInvoke();
        if (state == states.KICK)
            return;
        this.state = states.KICK;
        anim.Play("kick_1");
        
        Invoke("Reset", 0.25f);
    //    SetColliders(false);
    }
    public void Dash()
    {
        CancelInvoke();
        if (state == states.DASH)
            return;
        this.state = states.DASH;
        anim.Play("dash");
        character.ChangeSpeedTo(Data.Instance.settings.speedDash);

        Invoke("Reset", 0.25f);
    }
    void SetColliders(bool isOn)
    {
        foreach (Collider c in colliders)
            c.enabled = isOn;
    }
    private void Reset()
    {
        character.ChangeSpeedTo(0);
        state = states.ACTION_DONE;
        Idle();
        SetColliders(true);
    }
   
}
