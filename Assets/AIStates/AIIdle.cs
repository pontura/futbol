using UnityEngine;

public class AIIdle : AIState
{
    public override void Init(AI ai)
    {
        base.Init(ai);
        color = Color.green;
    }
    public override void SetActive()
    {
        base.SetActive();
        ai.character.actions.Idle();
        timer = 0;
    }
    public override AIState UpdatedByAI()
    {
        timer += Time.deltaTime;
        if (timer > 2)
            SetState(ai.aiPosition);
        return State();
    }
    public override void GotoBall()
    {
        SetState(ai.aiGotoBall);
    }
    public override void OnCharacterCatchBall(Character character)
    {
        if (character.data.id == ai.character.data.id)
            SetState(ai.aiHasBall);
        else
            SetState(ai.aiPosition);
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
