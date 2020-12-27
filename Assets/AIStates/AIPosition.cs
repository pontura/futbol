using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPosition : AIState
{    
    public Vector3 gotoPosition;  
    public bool isHelper; // lo sigue al qeu tiene la pelota
    
    public override void Init(AI ai)
    {
        gotoPosition = ai.originalPosition;
        base.Init(ai);
        color = Color.yellow;
    }
    public override void SetActive()
    {
        isHelper = false;
        timer = 0;
    }
    public override AIState UpdatedByAI()
    {
        if(timer == 0 || timer >0.65f)
            SetDestination();

        timer += Time.deltaTime;

        int _h, _v = 0;
        if (Vector3.Distance(ai.transform.position, gotoPosition) > 0.5f)
        {
            if (Mathf.Abs(ai.transform.position.x - gotoPosition.x) < 0.25f) _h = 0;
            else if (ai.transform.position.x < gotoPosition.x) _h = 1;
            else _h = -1;

            if (Mathf.Abs(ai.transform.position.z - gotoPosition.z) < 0.25f) _v = 0;
            else if (ai.transform.position.z < gotoPosition.z) _v = 1;
            else _v = -1;

            ai.character.SetPosition(_h, _v);
        }
        else if(isHelper) //no para:
        {
            SetDestination();
        }
        else
        {
            SetState(ai.aiIdle);
        }
        return State();
    }
    public override void OnFollow(Transform target)
    {
        SetState(ai.aiGotoBall);
    }
    public virtual void SetDestination()
    {
        timer = 0;
        if (ai.state == AI.states.DEFENDING)
            GetDefendPosition();
        else if (isHelper)
            UpdateAttackPositionHelper();
        else
            GetAttackPosition();

        SetLimits();
    }
    void GetDefendPosition()
    {
        gotoPosition = ai.originalPosition;
        gotoPosition.x += Utils.GetRandomFloatBetween(-2, 2);
        gotoPosition.z += Utils.GetRandomFloatBetween(-3, 3);
    }
    void GetAttackPosition()
    {
        CheckHelper();
        if (ai.character.type == Character.types.DELANTERO_UP || ai.character.type == Character.types.DELANTERO_DOWN)
            ai.character.SuperRun();
        gotoPosition = ai.originalPosition;
        float goto_x = Mathf.Abs(ai.originalPosition.x) - (Data.Instance.settings.gameplay.limits.x / 2) + Utils.GetRandomFloatBetween(0, 3);
        if (ai.character.teamID == 2)
            goto_x *= -1;
        gotoPosition.x = Mathf.Lerp(goto_x, ai.ball.transform.position.x, 0.5f);
        gotoPosition.z += Utils.GetRandomFloatBetween(-3, 3);
    }
    void UpdateAttackPositionHelper()
    {
        Vector3 characterWithBallPos = ai.characterWithBall.transform.position;

        if (ai.character.type == Character.types.DEFENSOR_DOWN || ai.character.type == Character.types.DEFENSOR_UP)
            ai.character.SuperRun();
        else if (Mathf.Abs(ai.transform.position.x- characterWithBallPos.x)>5)
            ai.character.SuperRun();

        float offset = GetOffsetToHelper();
        if (ai.character.teamID == 1)
            offset *= -1;

        gotoPosition.x = characterWithBallPos.x - offset;
        gotoPosition.z = ai.originalPosition.z + Utils.GetRandomFloatBetween(-2, 2);
    }
    void SetLimits()
    {
        if (Mathf.Abs(gotoPosition.z) > Data.Instance.settings.gameplay.limits.y / 2)
            gotoPosition.z = ai.originalPosition.z;
        if (Mathf.Abs(gotoPosition.x) > Data.Instance.settings.gameplay.limits.x / 2)
            gotoPosition.x = ai.originalPosition.x;
    }
    void CheckHelper()
    {
        isHelper = false;
        if (ai.characterWithBall == null)
            return;

        if (ai.characterWithBall.teamID == ai.character.teamID)
        {
            switch(ai.characterWithBall.type)
            {
                case Character.types.CENTRAL: break;
                case Character.types.GOALKEEPER:
                    if (ai.character.type == Character.types.CENTRAL) isHelper = true; break;
                case Character.types.DEFENSOR_DOWN:
                    if(ai.character.type == Character.types.DEFENSOR_UP) isHelper = true; break;
                case Character.types.DEFENSOR_UP:
                    if (ai.character.type == Character.types.DEFENSOR_DOWN) isHelper = true; break;
                case Character.types.DELANTERO_DOWN:
                    if (ai.character.type == Character.types.DELANTERO_UP) isHelper = true; break;
                case Character.types.DELANTERO_UP:
                    if (ai.character.type == Character.types.DELANTERO_DOWN) isHelper = true; break;
            }
        }
    }
    float GetOffsetToHelper()
    {
        if (ai.characterWithBall == null)
            return 0;
        switch (ai.characterWithBall.type)
        {
            case Character.types.CENTRAL: return 0;
            case Character.types.GOALKEEPER:
                return Random.Range(3, 6);
            case Character.types.DEFENSOR_DOWN:
                return Random.Range(0, 2);
            case Character.types.DEFENSOR_UP:
                return Random.Range(0, 2);
            case Character.types.DELANTERO_DOWN:
                return Random.Range(-1, 1);
            default:
                return Random.Range(-1, 1);
        }
    }
}
