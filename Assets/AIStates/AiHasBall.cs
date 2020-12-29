﻿using UnityEngine;

public class AiHasBall : AIState
{
    Vector3 dest;
    float center_goto_goal_x = 12;
    int _z = 0;
    Vector3 limits;

    public override void Init(AI ai)
    {
        limits = Data.Instance.settings.gameplay.limits;
        base.Init(ai);
        color = Color.red;       
    }
    public override void SetActive()
    {
        timer = 0;
        center_goto_goal_x = 12 + Utils.GetRandomFloatBetween(-4, 1);
        if (center_goto_goal_x < Mathf.Abs(ai.character.transform.position.x))
            center_goto_goal_x = Mathf.Abs(ai.character.transform.position.x) + 0.25f;
        dest = Vector3.zero;
        dest.x = center_goto_goal_x;
        if (ai.character.teamID == 1)
            dest.x *= -1;
        dest.z = Utils.GetRandomFloatBetween(0, 5);
        if (ai.character.transform.position.z < 0)
            dest.z *= -1;

        if (dest.z > limits.y / 2)
            dest.z = limits.y;
        else if (dest.z < -limits.y / 2)
            dest.z = -limits.y;

    }
    public override AIState UpdatedByAI()
    {
        timer += Time.deltaTime;
        if (timer > 0.5f)
            CheckPase();
        if (Mathf.Abs(ai.transform.position.x+0.25f) > center_goto_goal_x)
            KickBall();
        else
            RunToGoal();

        return State();
    }
    void RunToGoal()
    {
        if (timer == 0)
        {
            ai.character.SuperRun();
            if (Mathf.Abs(ai.transform.position.z - dest.z) < 3) _z = 0;
            else if (Random.Range(0,10)<5)  _z = 1;
            else  _z = -1;
        }

        int _x = 0;
        if (ai.transform.position.x < dest.x) _x = 1; else _x = -1;

        if (_x == 0 && _z == 0)
            KickBall();
        else
            ai.character.MoveTo(_x, _z);
    }
    void KickBall()
    {
        ai.character.Kick(CharacterActions.kickTypes.KICK_TO_GOAL);
        SetState(ai.aiPositionAttacking);
    }
    void CheckPase()
    {
        timer = 0;
        if (Random.Range(0, 10) < 5) return;
        Character characterToPass = Game.Instance.charactersManager.GetNearestTo(ai.character, ai.character.teamID);

        if (characterToPass == null) return;
        Vector3 otherPos = characterToPass.transform.position;

        //fuera de posicion de pase:
        if (ai.character.teamID == 2 && otherPos.x < ai.transform.position.x - 2
            || ai.character.teamID == 1 && otherPos.x > ai.transform.position.x + 2
            )return;

        float offset = 3;
        if (ai.character.teamID == 1)
            otherPos.x -= offset;
        else if (ai.character.teamID == 2)
            otherPos.x += offset;       

        ai.character.ballCatcher.LookAt(otherPos);
        CharacterActions.kickTypes kickType;

        //tipos de pase
        if (ai.character.teamID == 2 && otherPos.x > 10 || ai.character.teamID == 1 && otherPos.x < -10)
            ai.character.Kick(CharacterActions.kickTypes.CENTRO, Utils.GetRandomFloatBetween(0.5f, 2));
        else
            ai.character.Kick(CharacterActions.kickTypes.SOFT, Utils.GetRandomFloatBetween(0.5f,2));

        SetState(ai.aiIdle);
    }
}
