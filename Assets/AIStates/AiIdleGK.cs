using UnityEngine;
using System.Collections;

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
        if (timer > 0.5f)
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
            SetState(ai.aiAlertGK);
    }
    public override void OnBallNearOnAir()
    {
        ai.character.actions.Jump();
    }
    public override void OnFollow(Transform target)
    {
        SetState(ai.aiGotoBall);
    }
}
