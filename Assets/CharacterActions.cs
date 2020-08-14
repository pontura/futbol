using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    Animator anim;
    
    

    public states state;

    public enum states
    {
        IDLE,
        RUN,
        KICK,
        ACTION_DONE
    }

    public void Init(GameObject go, int teamID)
    {
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
    public void LookTo(int v )
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
        if (state == states.RUN || state == states.KICK)
            return;
        this.state = states.RUN;
        anim.Play("run");
    }
    public void Kick()
    {
        if (state == states.KICK)
            return;
        this.state = states.KICK;
        anim.Play("kick_1");
        
        Invoke("Reset", 0.25f);
    }
    private void Reset()
    {
        state = states.ACTION_DONE;
        Idle();
    }
   
}
