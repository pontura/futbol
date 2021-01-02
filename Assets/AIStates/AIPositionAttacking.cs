using UnityEngine;

public class AIPositionAttacking : AIState
{
    public Vector3 gotoPosition;
    public bool isHelper; // lo sigue al qeu tiene la pelota
    float delay;

    public override void Init(AI ai)
    {
        delay = Data.Instance.settings.gameplay.attackDelay;
        gotoPosition = ai.originalPosition;
        base.Init(ai);
        color = Color.grey;
    }
    public override void SetActive()
    {
        isHelper = false;
        timer = 0;
        SetDestination();
    }
    public override void OnCharacterCatchBall(Character character)
    {
        isHelper = false;
        if (character.data.id == ai.character.data.id)
            SetState(ai.aiHasBall);
        else if (character.teamID != ai.character.teamID)
            SetState(ai.aiPositionDefending);
    }
    public override AIState UpdatedByAI()
    {
        if (timer > delay)
            SetDestination();

        timer += Time.deltaTime;

        int _h, _v = 0;
        Vector3 pos = ai.transform.position;
        if (Vector3.Distance(pos, gotoPosition) > 0.5f)
        {
            if (Mathf.Abs(pos.x - gotoPosition.x) < 0.25f) _h = 0;
            else if (pos.x < gotoPosition.x) _h = 1;
            else _h = -1;

            if (Mathf.Abs(pos.z - gotoPosition.z) < 0.25f) _v = 0;
            else if (pos.z < gotoPosition.z) _v = 1;
            else _v = -1;

            ai.character.SetPosition(_h, _v);
        }
        else if (isHelper) //no para:
        {
            SetDestination();
        }
        else
        {
            SetState(ai.aiIdle);
        }
        return State();
    }
    public override void GotoBall()
    {
        SetState(ai.aiGotoBall);
    }
    public override void OnFollow(Transform target)
    {
        SetState(ai.aiGotoBall);
    }
    public virtual void SetDestination()
    {
        timer = 0;
        if (isHelper)
            UpdateAttackPositionHelper();
        else
            UpdateAttackPosition();
    }
    void UpdateAttackPosition()
    {
        CheckHelper();
        if (ai.character.type == Character.types.DELANTERO_UP || ai.character.type == Character.types.DELANTERO_DOWN && Random.Range(0, 10) < 5)
            ai.character.SuperRun();
        gotoPosition = ai.originalPosition;
        float resta = (Data.Instance.stadiumData.active.size_x / 2.25f) + Utils.GetRandomFloatBetween(-2, 2);
        if(ai.character.teamID == 1)
            gotoPosition.x = ai.originalPosition.x - resta;
        else
            gotoPosition.x = ai.originalPosition.x + resta;
        gotoPosition.x = Mathf.Lerp(gotoPosition.x, ai.ball.transform.position.x, 0.35f);
        gotoPosition.z += Utils.GetRandomFloatBetween(-3, 3);
    }
    void UpdateAttackPositionHelper()
    {
        Vector3 characterWithBallPos = ai.characterWithBall.transform.position;

        if (ai.character.type == Character.types.DEFENSOR_DOWN || ai.character.type == Character.types.DEFENSOR_UP)
            ai.character.SuperRun();
        else if (Mathf.Abs(ai.transform.position.x - characterWithBallPos.x) > 5)
            ai.character.SuperRun();

        float offset = GetOffsetToHelper();
        if (ai.character.teamID == 1)
            offset *= -1;

        gotoPosition.x = characterWithBallPos.x - offset;
        gotoPosition.z = ai.originalPosition.z + Utils.GetRandomFloatBetween(-2, 2);
    }
    void CheckHelper()
    {
        isHelper = false;
        if (ai.characterWithBall == null)
            return;

        if (ai.characterWithBall.teamID == ai.character.teamID)
        {
            switch (ai.characterWithBall.type)
            {
                case Character.types.CENTRAL: break;
                //case Character.types.GOALKEEPER:
                //    if (ai.character.type == Character.types.CENTRAL) isHelper = true; break;
                case Character.types.DEFENSOR_DOWN:
                    if (ai.character.type == Character.types.DEFENSOR_UP) isHelper = true; break;
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
