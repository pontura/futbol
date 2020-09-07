using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPositionGoalKeeper : AIPosition
{
    public GoalKeeper goalKeeper;
    public bool running;
    float speedToGoalKeeperJump = 10;

    void Update()
    {
        if (!running && ai.ball.character == null)
        {
            if (ai.character.teamID == 1 &&     ai.ball.transform.position.x > 14  && ai.ball.rb.velocity.x > 4
                || ai.character.teamID == 2 &&  ai.ball.transform.position.x < -14 && ai.ball.rb.velocity.x < -4
                )
            {
                ai.ResetAll();
                ai.aiGotoBall.Init();
            }
        }
        else if (running && ai.ball.character != null && ai.ball.character == ai.character)
            UpdateRunToKick();
        else
            UpdateY();
    }
    void UpdateGoalKeeping()
    {

    }
    void UpdateY()
    {
        Vector3 ballPos = ai.ball.transform.position;
        if (ballPos.x > 0 && ai.character.teamID == 1 || ballPos.x < 0 && ai.character.teamID == 2)
        {
            if ((int)transform.position.z < (int)ballPos.z)
                goalKeeper.MoveTo(0, 1);
            else if ((int)transform.position.z > (int)ballPos.z)
                goalKeeper.MoveTo(0, -1);
            else
                goalKeeper.actions.Idle();
        }
    }
    void UpdateRunToKick()
    {
        if (ai.character.teamID == 1)
            goalKeeper.MoveTo(-1, 0);
        else
            goalKeeper.MoveTo(1, 0);
    }
    public void CharacterCatchBall()
    {
        running = false;
        print("goal keepr has ball");
        CancelInvoke();
        this.enabled = true;
        Invoke("CheckToRun", Random.Range(1.5f, 4.5f));
    }
    void CheckToRun()
    {
        if (ai.ball.character != null && ai.ball.character == ai.character)
        {
            running = true;
            Invoke("CheckToThrowBall", Random.Range(0.5f, 1.5f));
        }
    }
    void CheckToThrowBall()
    {
        running = false;
        if (ai.ball.character != null && ai.ball.character == ai.character)
        {
            ai.character.Kick(CharacterActions.kickTypes.HARD);
        }
    }
}
