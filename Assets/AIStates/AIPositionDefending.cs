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

        int _h, _v = 0;
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
        float offset_x = Utils.GetRandomFloatBetween(2, 6);
        float offset_z = Utils.GetRandomFloatBetween(-2, 2);

        if (ai.character.type == Character.types.DEFENSOR)
        {
            if (rand < 8)
                ai.character.SuperRun();
        }
        if (ai.character.type == Character.types.CENTRAL && rand < 5)
            ai.character.SuperRun();

        gotoPosition = ai.character.Oponent.transform.position;
        gotoPosition.z += offset_z;
        if (ai.character.teamID == 2)
            gotoPosition.x -= offset_x;
        else
            gotoPosition.x += offset_x;
  
        
    }   
}
