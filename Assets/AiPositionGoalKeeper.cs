﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPositionGoalKeeper : AIPosition
{
    public GoalKeeper goalKeeper;
    public bool running;
    Vector3 speedToCatch= new Vector2(1,10);

    float speed = 0.15f;
    float timeToChange = 1;
    float frameValue;

    void ChangeSpeedRandom()
    {
        speed = (float)Random.Range(speedToCatch.x, speedToCatch.y) / 100;
    }
    public override void UpdatedByAI()
    {
        frameValue += Time.deltaTime;
        if (frameValue>timeToChange)
        {
            ChangeSpeedRandom();
            frameValue = 0;
        }
        if (!running && ai.ball.character == null && IsBallGoingToGoal())
            TryToCatch();
        else if (IsBallInsideArea())
            UpdateGotoBall();
        else if (goalKeeper.teamID == 2 && ai.ball.transform.position.x < transform.position.x)
            goalKeeper.MoveTo(-1, 0);
        else if (goalKeeper.teamID == 1 && ai.ball.transform.position.x > transform.position.x)
            goalKeeper.MoveTo(1, 0);
        else if (running && ai.ball.character != null && ai.ball.character == ai.character)
            UpdateRunToKick();
        else
            UpdateY();
    }
    bool IsBallGoingToGoal()
    {
        if (ai.character.teamID == 1 && ai.ball.transform.position.x > 14 && ai.ball.rb.velocity.x > 4
            || ai.character.teamID == 2 && ai.ball.transform.position.x < -14 && ai.ball.rb.velocity.x < -4
            )
            return true;
        return false;
    }
    bool IsBallInsideArea()
    {
        if (ai.ball.transform.position.z < goalKeeper.area_limits.y
           &&
           ai.ball.transform.position.z > -goalKeeper.area_limits.y
          )
        {
            if (goalKeeper.teamID == 1 && ai.ball.transform.position.x > goalKeeper.area_limits.x
                ||
                goalKeeper.teamID == 2 && ai.ball.transform.position.x < -goalKeeper.area_limits.x
               )
                return true;
        }
        return false;
    }
    private void TryToCatch()
    {
        goalKeeper.actions.GoalKeeperJump();
        Vector3 pos = transform.position;
        
        pos.z = Mathf.Lerp(pos.z, ai.ball.transform.position.z, speed);
        transform.position = pos;
    }
    private void UpdateGotoBall()
    {
        goalKeeper.actions.Run();
        Vector3 pos = transform.position;
        pos.x = Mathf.Lerp(pos.x, ai.ball.transform.position.x, 0.025f);
        pos.z = Mathf.Lerp(pos.z, ai.ball.transform.position.z, 0.1f);
        transform.position = pos;
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
