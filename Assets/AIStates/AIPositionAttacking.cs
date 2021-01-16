using UnityEngine;

public class AIPositionAttacking : AIState
{
    public Vector3 gotoPosition;
    public bool isHelper; // lo sigue al qeu tiene la pelota
    float delay;
    public bool goalkeeperHasBall;
    Vector3 ballPos;

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
        if (ai.ball.character != null && ai.ball.character.type == Character.types.GOALKEEPER)
            goalkeeperHasBall = true;
        else
            goalkeeperHasBall = false;
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

        ballPos = ai.ball.transform.position;

        if (isHelper)
            UpdateAttackPositionHelper();
        else
            UpdateAttackPosition();
    }
    void UpdateAttackPosition()
    {
        CheckHelper();
        
        if (ai.character.type == Character.types.DELANTERO && Random.Range(0, 10) < 5)
            ai.character.SuperRun();
  
        gotoPosition = ai.originalPosition;

        float resta = (Data.Instance.stadiumData.active.size_x / 2.25f) + Utils.GetRandomFloatBetween(-2, 2);

        //si la agarra tu arquero y sos defensor, vas a buscarla:
        if (goalkeeperHasBall &&
        ai.character.type == Character.types.DEFENSOR)
            resta -= (Data.Instance.stadiumData.active.size_x / 3);

        if (ai.character.teamID == 1)
            gotoPosition.x = ai.originalPosition.x - resta;   
        else
            gotoPosition.x = ai.originalPosition.x + resta;

        if (ai.character.type == Character.types.DEFENSOR)
        {
            gotoPosition.x = Mathf.Lerp(ai.originalPosition.x, ai.ball.transform.position.x, 0.5f);
        }
        else if (ai.character.type == Character.types.CENTRAL)
        {
            gotoPosition.x = Mathf.Lerp(ai.originalPosition.x, ai.ball.transform.position.x, 0.7f);
        }
        else
        {
            if (Random.Range(0, 10) < 5) // considera a veces la posicion adelantada
            {
                float posicionAdelantada = ai.character.charactersManager.GetPosicionAdelantada(ai.character.teamID);
                if (gotoPosition.x < posicionAdelantada) gotoPosition.x = posicionAdelantada; // para no quedar adelantados
            }
            gotoPosition.x = Mathf.Lerp(gotoPosition.x, ballPos.x, 0.35f);            
        }
        gotoPosition.z += Utils.GetRandomFloatBetween(-3, 3);

        if (Vector3.Distance(gotoPosition, ballPos)<4)
            Opposite_Z();
    }
    void UpdateAttackPositionHelper()
    {   
       // if (ai.character.type == Character.types.DEFENSOR_DOWN || ai.character.type == Character.types.DEFENSOR_UP)
            ai.character.SuperRun();
       // else if (Mathf.Abs(ai.transform.position.x - characterWithBallPos.x) > 5)
        //    ai.character.SuperRun();

        float offset = GetOffsetXToHelper();
        if (ai.character.teamID == 1)
            offset *= -1;
      
        gotoPosition.x = ballPos.x - offset;

        float posicionAdelantada = ai.character.charactersManager.GetPosicionAdelantada(ai.character.teamID);
        if (gotoPosition.x < posicionAdelantada) gotoPosition.x = posicionAdelantada; // para no quedar adelantados
        gotoPosition.z = ai.originalPosition.z + Utils.GetRandomFloatBetween(-2, 2);
        Opposite_Z();
    }
    void Opposite_Z()
    {
        if ((ballPos.z > 0 && gotoPosition.z > 0) || (ballPos.z < 0 && gotoPosition.z < 0))
            gotoPosition.z *= -1;
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
                case Character.types.CENTRAL:
                    if (ai.character.type == Character.types.DELANTERO)
                    {
                        if(ai.character.fieldPosition == Character.fieldPositions.UP && ai.characterWithBall.transform.position.z<0)
                            isHelper = true;
                        else if (ai.character.fieldPosition == Character.fieldPositions.DOWN && ai.characterWithBall.transform.position.z > 0)
                            isHelper = true;
                    }
                    break;
                case Character.types.DEFENSOR:
                    if (FarFromOrigin() > 20)
                        if (ai.character.type == Character.types.DELANTERO)
                        {
                            if (ai.character.fieldPosition == Character.fieldPositions.UP && ai.characterWithBall.transform.position.z < 0)
                                isHelper = true;
                            else if (ai.character.fieldPosition == Character.fieldPositions.DOWN && ai.characterWithBall.transform.position.z > 0)
                                isHelper = true;
                        }
                        else
                        if (ai.character.type == Character.types.DEFENSOR)
                        {
                            if (ai.character.fieldPosition == Character.fieldPositions.UP && ai.characterWithBall.transform.position.z < 0)
                                isHelper = true;
                            else if (ai.character.fieldPosition == Character.fieldPositions.DOWN && ai.characterWithBall.transform.position.z > 0)
                                isHelper = true;
                        }
                    break;
                case Character.types.DELANTERO:
                    if (ai.character.type == Character.types.DELANTERO)
                    {
                        if (ai.character.fieldPosition == Character.fieldPositions.UP && ai.characterWithBall.transform.position.z < 0)
                            isHelper = true;
                        else if (ai.character.fieldPosition == Character.fieldPositions.DOWN && ai.characterWithBall.transform.position.z > 0)
                            isHelper = true;
                    }
                    break;
            }
        }
    }
    float FarFromOrigin()
    {
        return Mathf.Abs(ballPos.x - ai.originalPosition.x);
    }
    float GetOffsetXToHelper()
    {
        if (ai.characterWithBall == null)
            return 0;
        switch (ai.characterWithBall.type)
        {
            case Character.types.CENTRAL: return Random.Range(-4, 4);
            case Character.types.GOALKEEPER:
                return Random.Range(3, 6);
            case Character.types.DEFENSOR:
                return Random.Range(-1, 3);
            case Character.types.DELANTERO:
                return Random.Range(-3, 3);
            default:
                return Random.Range(-3, 3);
        }
    }
}
