using UnityEngine;

public class AIPositionDefending : AIState
{
    public Vector3 gotoPosition;
    float delay; 

    public override void Init(AI ai)
    {
        delay = Data.Instance.settings.gameplay.defenseDelay;
        gotoPosition = ai.originalPosition;
        base.Init(ai);
        color = Color.white;     
    }
    public override void SetActive()
    {
        timer = 0;
        SetDestination();
    }
    public override void OnCharacterCatchBall(Character character)
    {
        if (character.data.id == ai.character.data.id)
            SetState(ai.aiHasBall);
        else if (character.teamID == ai.character.teamID)
            SetState(ai.aiPositionAttacking);
    }
    public override AIState UpdatedByAI()
    {
        if (timer > delay)
            SetDestination();

        timer += Time.deltaTime;

        int _h = 0;
        int _v = 0;
        Vector3 pos = ai.transform.position;
       // gotoPosition = ai.character.SetPositionInsideLimits(gotoPosition);

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
        UpdatePosition();
    }
    void UpdatePosition()
    {
        int rand = Random.Range(0, 10);
        float offset_z = Utils.GetRandomFloatBetween(-2, 2);

        if (ai.character.type == Character.types.DEFENSOR)
        {
            if (rand < 8)
                ai.character.SuperRun();
        }
        if (ai.character.type == Character.types.CENTRAL && rand < 5)
            ai.character.SuperRun();

        Vector3 ballPos = ai.ball.transform.position;

        float diffPos = 0.2f;     
        
        if (ai.character.type == Character.types.DEFENSOR)
            diffPos = 0.55f;
        else if (ai.character.type == Character.types.CENTRAL)
            diffPos = 0.6f;
        else if (ai.character.type == Character.types.DELANTERO)
            diffPos = 0.8f;

        if (Mathf.Sign(ballPos.x) == Mathf.Sign(ai.originalPosition.x)) // si está de tu lado la pelota se posiciona más cerca de su origen
            diffPos /= 4;

        gotoPosition = Vector3.Lerp(ai.originalPosition, ai.character.oponent.transform.position, diffPos);
        gotoPosition.z += offset_z;
        
    }   
}
