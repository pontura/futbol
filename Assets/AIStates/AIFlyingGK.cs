using UnityEngine;

public class AIFlyingGK: AIState
{   
    float timeToChange = 1f;
    float flyingDuration;

    public override void Init(AI ai)
    {
        base.Init(ai);
        color = Color.red;
    }
    public override void SetActive()
    {
        Debug.Log("_________FFLY");
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
            int _x = 0; int _z = 0;
            Vector3 ballPos = ai.ball.transform.position;
            Vector3 dest = ai.GetPositionInsideArea(ballPos);
            if (ai.character.transform.position.x > dest.x + 0.5f) _x = -1;
            else if (ai.character.transform.position.x < dest.x - 0.5f) _x = 1;
            if (ai.character.transform.position.z > dest.z + 0.5f) _z = -1;
            else if (ai.character.transform.position.z < dest.z - 0.5f) _z = 1;

            ai.character.MoveTo(_x, _z);
        }
        return State();
    }
    
}
