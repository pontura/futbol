using UnityEngine;

public class AIHasBallGK : AIState
{
    float areaLimits_x = 4;
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
    public override void OnCharacterCatchBall(Character character)
    {
        if (character.data.id == ai.character.data.id)
            SetState(ai.aiHasBallGK);
        else if (character.teamID == ai.character.teamID)
            SetState(ai.aiPositionGK);
        else
            SetState(ai.aiIdleGK);
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
        if (IsOutsideAreaInX(ai.character.transform.position.x) || timer>1.5f)
        {
            CheckSaque();
            timer = 0;
            state = states.KICKED;
        }
        ai.character.MoveTo(direction, 0);
    }
    bool IsOutsideAreaInX(float _x)
    {
        if (ai.character.teamID == 1 && ai.character.transform.position.x < (ai.originalPosition.x - areaLimits_x)) return true;
        if (ai.character.teamID == 2 && ai.character.transform.position.x > (ai.originalPosition.x + areaLimits_x)) return true;
        return false;
    }
    void CheckSaque()
    {
        int rand = Random.Range(0, 9);
        if (rand < 3)
        {
            ai.character.Kick(CharacterActions.kickTypes.HARD, Utils.GetRandomFloatBetween(1.55f, 3.3f));
            return;
        }

        Character characterToPass = Game.Instance.charactersManager.GetNearestTo(ai.character, ai.character.teamID);

        if (characterToPass == null) return;
        Vector3 otherPos = characterToPass.transform.position;

        ai.character.ballCatcher.LookAt(otherPos);

        if (Random.Range(0,10)<10 || ai.character.teamID == 2 && otherPos.x > 10 || ai.character.teamID == 1 && otherPos.x < -10)
            ai.character.Kick(CharacterActions.kickTypes.CENTRO, Utils.GetRandomFloatBetween(2.75f, 4.5f));
        else
            ai.character.Kick(CharacterActions.kickTypes.SOFT, Utils.GetRandomFloatBetween(1.5f, 3));
    }
}