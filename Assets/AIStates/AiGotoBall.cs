using UnityEngine;

public class AiGotoBall : AIState
{
    Vector3 dest;
    float offset;
    float timerToCalculate = 0.3f;

    public override void Init(AI ai)
    {
        base.Init(ai);
        color = Color.black;
       
    }
    public override void SetActive()
    {
        if (ai.character.teamID == 1)
            offset = 0.25f;
        else
            offset = -0.25f;
        timer = 0;
        SetDest();
    }
    public override AIState UpdatedByAI()
    {
        timer += Time.deltaTime;
        if (timer > timerToCalculate)
            SetDest();
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
        else if (character.teamID == ai.character.teamID)
            SetState(ai.aiPositionAttacking);
        else
            SetState(ai.aiIdle);
    }
    void SetDest()
    {
        timer = 0;
        dest = ai.ball.transform.position;
        dest.x += offset;
        if (ai.ball.character != null && ai.ball.character.type == Character.types.GOALKEEPER)
        {
            SetState(ai.aiPositionDefending);
            return;
        }
        if (ai.character.actions.state == CharacterActions.states.DASH || ai.character.actions.runFast)
            return;
        float distToBall = Vector3.Distance(ai.transform.position, dest);
        if (distToBall > 20)
            ai.character.SuperRun();
        else if (distToBall < distance_to_dash_ai && Random.Range(0, 100) < random_dash_percent)
            ai.character.Dash();
        else if (
            ai.character.teamID == 2 && ai.character.transform.position.x > dest.x
           || ai.character.teamID == 1 && ai.character.transform.position.x < dest.x)
            ai.character.SuperRun();
    }

}
