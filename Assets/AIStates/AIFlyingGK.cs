using UnityEngine;

public class AIFlyingGK: AIState
{   
    float timeToChange = 1;
    float flyingDuration;

    public override void Init(AI ai)
    {
        base.Init(ai);
        color = Color.red;
    }
    public override void SetActive()
    {
        flyingDuration = ai.character.stats.gk_FlyingDuration;
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
        if (timer < flyingDuration)
        {
            Vector3 dest = ai.ball.transform.position;
            dest.y = ai.transform.position.y;
            ai.transform.position = Vector3.Lerp(ai.transform.position, ai.GetPositionInsideArea(dest), ai.character.stats.gk_FlyingSpeed/10);
        }
        return State();
    }
    
}
