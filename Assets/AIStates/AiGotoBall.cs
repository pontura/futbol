using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiGotoBall : AIState
{
    Vector3 dest;
    public override void Init(AI ai)
    {
        base.Init(ai);
        color = Color.black;
    }
    public override void SetActive()
    {
        timer = 0;
    }
    public override AIState UpdatedByAI()
    {
        timer += Time.deltaTime;
        if (timer > 0.85f)
            CheckRunType();
        int _x = 0;
        int _z = 0;
        if (Mathf.Abs(ai.transform.position.x - dest.x) > 0.15f)
        {
            if (ai.transform.position.x < dest.x) _x = 1; else _x = -1;
        }
        if (ai.transform.position.z < dest.z)  _z = 1;  else _z = -1;
        if ((_x == 0 && _z == 0) || (ai == null || ai.character == null))
            SetState( ai.aiIdle);

        ai.character.MoveTo(_x, _z);
        return State();
    }
    public override void OnCharacterCatchBall(Character character)
    {
        if (character.data.id == ai.character.data.id)
            SetState(ai.aiHasBall);
        else
            SetState(ai.aiPosition);
    }
    void CheckRunType()
    {
        timer = 0;
        dest = ai.ball.transform.position;
        float distToBall = Vector3.Distance(ai.transform.position, dest);
        if (distToBall > 20)
            ai.character.SuperRun();
        else if (distToBall < 4 && Random.Range(0, 10) < 7)
            ai.character.Dash();
        else if (
            ai.character.teamID == 1 && ai.character.transform.position.x > dest.x
           || ai.character.teamID == 2 && ai.character.transform.position.x < dest.x)
            ai.character.SuperRun();
    }

}
