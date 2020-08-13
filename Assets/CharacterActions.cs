using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActions : MonoBehaviour
{
    public Animator anim;
    public float kickForce = 1000;
    Ball ball;

    public states state;

    public enum states
    {
        IDLE,
        RUN,
        KICK,
        ACTION_DONE
    }
    private void Start()
    {
        lookTo = (int)-transform.localScale.x;
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
        if (ball != null)
        {
            Vector2 direction = -transform.right;
            if (transform.localScale.x<0)
                direction = transform.right;
            Vector3 dir = direction * kickForce;
            dir += Vector3.up * kickForce / 4;
            ball.rb.AddForce(dir);
        }
        Invoke("Reset", 0.25f);
    }
    private void Reset()
    {
        state = states.ACTION_DONE;
        Idle();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ball")
        {
            ball = other.GetComponent<Ball>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ball")
        {
            ball = null;
        }
    }
}
