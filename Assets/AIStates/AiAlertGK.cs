﻿using UnityEngine;
using System.Collections;

public class AiAlertGK : AIState
{
    float areaLimits_x = 3;
    float areaLimits_z = 5;

    float ball_distance_to_go = 6;
    float smoothSale = 0.05f;

    public override void Init(AI ai)
    {       
        base.Init(ai);
        color = Color.blue;
    }
    public override void OnCharacterCatchBall(Character character)
    {
        if (character.data.id == ai.character.data.id)
            SetState(ai.aiHasBallGK);
        else if (character.teamID == ai.character.teamID)
            SetState(ai.aiPositionGK);
    }
    public override void OnBallNearOnAir()
    {
        SetState(ai.aiFlyingGK);
    }
    float _x = 0;
    float _z = 0;
    public override AIState UpdatedByAI()
    {
        _x = 0;
        _z = 0;

        if (ai.ball.character != null && ai.ball.character == ai.character)
            SetState(ai.aiHasBallGK);

        Vector3 ballPos = ai.ball.transform.position;
        
        float diff_X = Mathf.Abs(ai.transform.position.x - ballPos.x);
        float diff_Z = Mathf.Abs(ai.transform.position.z - ballPos.z);

        if (diff_Z > 0.15f)
            if (ai.transform.position.z > ballPos.z) _z = -1; else _z = 1;
     

        if (ai.transform.position.x < ai.ball.transform.position.x) _x = 1;
        else if (ai.transform.position.x > ai.ball.transform.position.x) _x = -1;

        //limites del area:
        if (IsOutsideAreaInX()) _x = 0;
        if (IsOutsideAreaInZ()) _z = 0;

        if (Vector3.Distance(ai.transform.position,  ballPos) < ball_distance_to_go)
            UpdateInArea();

        ai.character.MoveTo(_x, _z);
        return State();
    }
    void UpdateInArea()
    {
        Vector3 dest = ai.ball.transform.position;
        dest.y = ai.transform.position.y;
        ai.transform.position = Vector3.Lerp(ai.transform.position, ai.GetPositionInsideArea(dest), smoothSale);
    }
  
    bool IsOutsideAreaInX()
    {
        if (ai.character.teamID == 1 && _x<0 && ai.transform.position.x < (ai.originalPosition.x - areaLimits_x)) return true;
        if (ai.character.teamID == 2 && _x>0 && ai.transform.position.x > (ai.originalPosition.x + areaLimits_x)) return true;
        return false;
    }
    bool IsOutsideAreaInZ()
    {
        if (_z == -1 && ai.transform.position.z < -areaLimits_z) return true;
        if (_z == 1 && ai.transform.position.z > areaLimits_z) return true;
        return false;
    }

}
