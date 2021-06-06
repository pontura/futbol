using UnityEngine;

public class AiIdleGK : AIState
{
    public override void Init(AI ai)
    {
        Debug.Log("Init AiIdleGK");
        base.Init(ai);
        color = Color.green;
    }
    public override void SetActive()
    {
        base.SetActive();
        ai.character.actions.Idle();
    }
    public override AIState UpdatedByAI()
    {
        timer += Time.deltaTime;
        float distToBall = Vector3.Distance(ai.ball.transform.position, ai.transform.position);
        if (distToBall < 5)
        {
            if(IsBallComingToGoal(distToBall))
                SetState(ai.aiFlyingGK);
            else
                SetState(ai.aiAlertGK);
        }
        else if (timer > 0.5f)
        {
            timer = 0;
            SetState(ai.aiPositionGK);
        } else if (ai.ball.character != null && ai.ball.character == ai.character)
            SetState(ai.aiHasBallGK);
        return State();
    }
    public override void OnCharacterCatchBall(Character character)
    {
        if (character.data.id == ai.character.data.id)
            SetState(ai.aiHasBallGK);
        else if (character.teamID== ai.character.teamID)
            SetState(ai.aiPositionGK);
        else
            SetState(ai.aiIdleGK);
    }
    public override void OnBallNearOnAir()
    {
        ai.character.actions.Jump();
    }
    public override void OnFollow(Transform target)
    {
        SetState(ai.aiGotoBall);
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
