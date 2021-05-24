using UnityEngine;

public class AIIdle : AIState
{
    float initTimer;
    AIState aiNextState;

    public override void Init(AI ai)
    {
        base.Init(ai);
        color = Color.green;
    }
    public override void SetActive()
    {
        aiNextState = null;
        initTimer = Time.time;
        base.SetActive();
        ai.character.actions.Idle();
        timer = 0;
    }
    public override AIState UpdatedByAI()
    {
        initTimer += Time.deltaTime;
        timer += Time.deltaTime;
        if (timer > 0.75f)
        {
            if (initTimer >0.5f && aiNextState != null)
            {
                SetState(aiNextState);
                return State();
            }
            timer = 0;
            if (ai.ball.character == null)
            {
                Vector3 ballPos = ai.ball.transform.position;
                if (Mathf.Sign(ballPos.x) != Mathf.Sign(ai.originalPosition.x))
                {
                    if(ai.character.type == Character.types.DELANTERO)
                    {
                        if(ballPos.z>0 && ai.character.fieldPosition == Character.fieldPositions.UP)
                            SetState(ai.aiGotoBall);
                        else if (ballPos.z < 0 && ai.character.fieldPosition == Character.fieldPositions.DOWN)
                            SetState(ai.aiGotoBall);
                        else
                            SetState(ai.aiPositionAttacking);
                    }                        
                }
                else
                {
                    if (ai.character.type == Character.types.DEFENSOR)
                    {
                        if (ballPos.z > 0 && ai.character.fieldPosition == Character.fieldPositions.UP)
                            SetState(ai.aiGotoBall);
                        else if (ballPos.z < 0 && ai.character.fieldPosition == Character.fieldPositions.DOWN)
                            SetState(ai.aiGotoBall);
                    }                        
                }
            } else
            {
                if (ai.ball.character.teamID == ai.character.teamID)
                    SetState(ai.aiPositionAttacking);
                else SetState(ai.aiPositionDefending);
            }               
        }            
        return State();
    }
    void SetNextState(AIState aistate)
    {
        if (initTimer < 0.75f)
            aiNextState = aistate;
        else
            SetState(aistate);
    }
    public override void GotoBall()
    {
        SetNextState(ai.aiGotoBall);
    }
    public override void OnCharacterCatchBall(Character character)
    {
        if (character.data.id == ai.character.data.id)
            SetState(ai.aiHasBall);
        else if (character.teamID == ai.character.teamID)
            SetNextState(ai.aiPositionAttacking);
        else
            SetNextState(ai.aiPositionDefending);
    }
    public override void OnBallNearOnAir()
    {
        ai.character.actions.Jump();
    }
    public override void OnFollow(Transform target)
    {
        SetNextState(ai.aiGotoBall);
    }
}
