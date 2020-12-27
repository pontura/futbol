using UnityEngine;
using System.Collections;

public class AIHasBallGK : AIState
{
    float areaLimits_x = 5;
    float direction;
    states state;

    enum states {
        IDLE,
        WALKING,
        KICKED,
        DONE
    }
    public override void Init(AI ai)
    {
        base.Init(ai);
        color = Color.yellow;
    }
    public override void SetActive()
    {
        state = states.IDLE;
        if (ai.character.teamID == 1)  direction = -1;  else  direction = 1;
        base.SetActive();
        timer = 0;
    }
    public override AIState UpdatedByAI()
    {
        timer += Time.deltaTime;

        if (state == states.IDLE && timer > 1)
            state = states.WALKING;
        else if (state == states.WALKING)
            UpdateWalking();
        else if (state == states.KICKED && timer > 1f)
        {
            state = states.DONE;
            SetState(ai.aiIdleGK);
        }            
        return State();
    }
    void UpdateWalking()
    {
        if (IsOutsideAreaInX(ai.character.transform.position.x))
        {
            ai.character.Kick(CharacterActions.kickTypes.BALOON);
            timer = 0;
            state = states.KICKED;
        }
        ai.character.MoveTo(direction, 0);
    }
    bool IsOutsideAreaInX(float _x)
    {
        if (ai.character.teamID == 1 && _x < (ai.originalPosition.x - areaLimits_x)) return true;
        if (ai.character.teamID == 2 && _x > (ai.originalPosition.x + areaLimits_x)) return true;
        return false;
    }
}