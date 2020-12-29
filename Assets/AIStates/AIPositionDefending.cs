using UnityEngine;

public class AIPositionDefending : AIState
{
    public Vector3 gotoPosition;
    Transform oponentTransform;
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
        if (oponentTransform == null)
            GetOponent();

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
        if (ai.character.type == Character.types.DEFENSOR_DOWN  || ai.character.type == Character.types.DEFENSOR_UP && Random.Range(0,10)<5)
            ai.character.SuperRun();

        gotoPosition = oponentTransform.position;

        float offset_x = Utils.GetRandomFloatBetween(0.5f, 2);
        float offset_z = Utils.GetRandomFloatBetween(-3, 3);

        if(ai.character.teamID == 2)
            gotoPosition.x -= offset_x;
        else
            gotoPosition.x += offset_x;

        gotoPosition.z += offset_z;
    }   
    Transform GetOponent()
    {
        int oponentTeam;
        if (ai.character.teamID == 1) oponentTeam = 2; else oponentTeam = 1;
        Character.types type = ai.character.type;

        if (type == Character.types.DEFENSOR_DOWN) type = Character.types.DELANTERO_DOWN;
        else if (type == Character.types.DEFENSOR_UP) type = Character.types.DELANTERO_UP;
        else if (type == Character.types.CENTRAL) type = Character.types.CENTRAL;
        else if (type == Character.types.DELANTERO_UP) type = Character.types.DEFENSOR_UP;
        else if (type == Character.types.DELANTERO_DOWN) type = Character.types.DEFENSOR_DOWN;

        oponentTransform = Game.Instance.charactersManager.GetCharacterByType(type, oponentTeam).transform;
        return oponentTransform;
    }
}
