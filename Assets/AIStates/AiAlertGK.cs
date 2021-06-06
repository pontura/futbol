using UnityEngine;

public class AiAlertGK : AIState
{
    float areaLimits_x = 1;
    float areaLimits_z = 6;

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
        else SetState(ai.aiIdleGK);
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
        float distanceToBall = Vector3.Distance(ai.transform.position, ballPos);
        if (distanceToBall < ball_distance_to_go)
        {
            Vector3 dest = ai.GetPositionInsideArea(ballPos);
            if (ai.character.transform.position.x > dest.x+0.1f) _x = -1;
            else if (ai.character.transform.position.x < dest.x-0.1f) _x = 1;
            if (ai.character.transform.position.z > dest.z + 0.1f) _z = -1;
            else if (ai.character.transform.position.z < dest.z - 0.1f) _z = 1;
            if (IsBallComingToGoal(distanceToBall))
                SetState(ai.aiFlyingGK);
        } else  {
            float diff_Z = Mathf.Abs(ai.transform.position.z - ballPos.z);

            if (diff_Z > 0.15f)
                if (ai.transform.position.z > ballPos.z) _z = -1; else _z = 1;

            if (ai.character.teamID == 1 && ai.transform.position.x < ai.originalPosition.x)
                _x = 1;
            else if (ai.character.teamID == 2 && ai.transform.position.x > ai.originalPosition.x)
                _x = -1;
            else _x = 0;
        }

        ai.character.MoveTo(_x, _z);

        return State();
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
    public bool IsBallComingToGoal(float distanceToBall)
    {
        if (distanceToBall < 12)
        {
            float ballSpeed = Mathf.Abs(ai.ball.rb.velocity.x);

            if (ballSpeed > 9
                &&
                (ai.character.teamID == 2 && Mathf.Abs(ai.ball.rb.velocity.x) < 0
                || (ai.character.teamID == 1 && Mathf.Abs(ai.ball.rb.velocity.x) > 0))
                )
            {
                return true;
            }
        }
        return false;
    }

}
