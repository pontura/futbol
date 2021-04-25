using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeperActions : CharacterActions
{
    public override void Idle()
    {
        if (state == states.SPECIAL_ACTION || state == states.IDLE || state == states.KICK)
            return;
        this.state = states.IDLE;
        if (Game.Instance.ball.character == character)
            anim.Play("idle_ball");
        else
            anim.Play("idle");
        LookAtBall();
    }
    public override void Run()
    {
        if (state == states.SPECIAL_ACTION || state == states.RUN || state == states.KICK || state == states.DASH)
            return;
        this.state = states.RUN;
        if (Game.Instance.ball.character == character)
            anim.Play("run_ball");
        else
            anim.Play("run");
    }
}
