using UnityEngine;

public class AiPositionGK : AIState
{
    float areaLimits_z = 6;

    public override void Init(AI ai)
    {
        base.Init(ai);
        color = Color.white;
    }

    public override AIState UpdatedByAI()
    {
        float _x = 0;
        float _z = 0;
        Vector3 ballPos = ai.ball.transform.position;
        ballPos.y = 0;
        float diff_X;

        float dest_z = ballPos.z / 1.8f;

        float diff_Z = Mathf.Abs(ai.transform.position.z - dest_z);

        if (diff_Z > 0.15f)
            if (ai.transform.position.z > dest_z) _z = -1; else _z = 1;  

        //limites del area:
        if (_z > 0 && ai.transform.position.z >= areaLimits_z)
            _z = 0;
        else if (_z < 0 && ai.transform.position.z <= -areaLimits_z)
            _z = 0;

        if (ai.character.teamID == 1 && ai.transform.position.x < ai.originalPosition.x)
            _x = 1;
        else if (ai.character.teamID == 2 && ai.transform.position.x > ai.originalPosition.x)
            _x = -1;
        else _x = 0;

        float distanceToBall = Vector3.Distance(ballPos, ai.transform.position);
        if (IsBallComingToGoal(distanceToBall))
            SetState(ai.aiFlyingGK);
        if (distanceToBall < Data.Instance.settings.gkSpeed_sale_x)
            SetState(ai.aiAlertGK);
        else if (_x == 0 && _z == 0)
            SetState(ai.aiIdleGK);
        else
            ai.character.MoveTo(_x, _z);
 
        return State();
    }
    public override void OnCharacterCatchBall(Character character)
    {
        if (character.data.id == ai.character.data.id)
            SetState(ai.aiHasBallGK);
        else if (character.teamID == ai.character.teamID)
            SetState(ai.aiPositionGK);
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
