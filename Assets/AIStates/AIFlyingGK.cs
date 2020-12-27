using UnityEngine;
using System.Collections;

public class AIFlyingGK: AIState
{   
    float smoothSale = 0.1f;
    float timeToChange = 1;
    float stopMovingTime = 0.5f;

    public override void Init(AI ai)
    {
        base.Init(ai);
        color = Color.red;
    }
    public override void SetActive()
    {
        timer = 0;
        ai.character.actions.GoalKeeperJump();
    }
    public override AIState UpdatedByAI()
    {
        timer += Time.deltaTime;
        if (timer > timeToChange)
        {
            timer = 0;
            SetState(ai.aiIdleGK);
        }
        if (timer < stopMovingTime)
        {
            Vector3 dest = ai.ball.transform.position;
            dest.y = ai.transform.position.y;
            ai.transform.position = Vector3.Lerp(ai.transform.position, ai.GetPositionInsideArea(dest), smoothSale);
        }
        return State();
    }
    
}
