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
        //timer += Time.deltaTime;
        //if (timer > 1)
        //    SetState(ai.aiIdleGK);

        float _x = 0;
        float _z = 0;
        Vector3 ballPos = ai.ball.transform.position;
        float diff_X;
        // gotoPosition = ai.character.SetPositionInsideLimits(gotoPosition);
        //if (ai.transform.position.x > ai.originalPosition.x)
        //        _x = -1;
        //    else _x = 1;


        float diff_Z = Mathf.Abs(ai.transform.position.z - ballPos.z);

        if (diff_Z > 0.15f)
            if (ai.transform.position.z > ballPos.z) _z = -1; else _z = 1;  

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

        if (_x == 0 && _z == 0)
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
        else
            SetState(ai.aiAlertGK);
    }
}
