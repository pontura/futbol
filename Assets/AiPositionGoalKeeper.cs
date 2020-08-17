using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPositionGoalKeeper : AIPosition
{
    public Ball ball;
    public GoalKeeper goalKeeper;

    void Update()
    {
        if (ai.state == AI.states.ATTACKING)
        {

        }  else
        {
            if (Mathf.Abs(transform.position.z - ball.transform.position.z) < 0.15f)
                goalKeeper.actions.Idle();
            else if (transform.position.z < ball.transform.position.z)
                goalKeeper.MoveTo(0, 1);
            else if (transform.position.z > ball.transform.position.z)
                goalKeeper.MoveTo(0, -1);
        }
    }
    public override void SetActive()
    {
        this.enabled = true;
    }
}
