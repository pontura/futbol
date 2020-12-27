using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiHasBall : AIState
{
    Vector3 dest;
    public Character characterToPass;
    float center_goto_goal_x = 15;
    int _z = 0;

    public override void Init(AI ai)
    {
        base.Init(ai);
        color = Color.red;
        characterToPass = null;
        timer = Time.time;
        dest = Vector3.zero;
        dest.x = center_goto_goal_x;
        if (ai.character.teamID == 1)
            dest.x *= -1;
    }
    public override AIState UpdatedByAI()
    {
        timer += Time.deltaTime;
        if (timer > 0.85f)
            Loop();

        if (characterToPass == null && Mathf.Abs(ai.transform.position.x - dest.x) < 1)
            KickBall();
        else
        {
            int _x = 0;

            if (ai.transform.position.x < dest.x) _x = 1; else _x = -1;
            if (Mathf.Abs(ai.transform.position.z - dest.z) > 3f)
            {
                if (ai.transform.position.z < dest.z) _z = 1; else _z = -1;
            }
            if (_x == 0 && _z == 0)
                SetState(ai.aiIdle);
            ai.character.MoveTo(_x, _z);
        }
        return State();
    }
    void Loop()
    {
        timer = 0;
        if (characterToPass == null && timer + 1 > Time.time && Random.Range(0,10)<4)
            IfNearGiveBall();
        
        int rand = Random.Range(0, 100);
        if (rand < 30)
            _z = 1;
        else if (rand < 60)
            _z = -1;
        else
            _z = 0;

        if(rand<70)
            ai.character.SuperRun();
    }
    public override void OnReset()
    {
        characterToPass = null;
    }
    void KickBall()
    {
        ai.character.Kick(CharacterActions.kickTypes.KICK_TO_GOAL);
        SetState(ai.aiPosition);
    }
    
    void IfNearGiveBall()
    {
        characterToPass = Game.Instance.charactersManager.GetNearestTo(ai.character, ai.character.teamID);
        Vector3 otherPos = characterToPass.transform.position;

        //fuera de posicion de pase:
        if (ai.character.teamID == 2 && otherPos.x < ai.transform.position.x - 2
            || ai.character.teamID == 1 && otherPos.x > ai.transform.position.x + 2
            )
        {
            characterToPass = null;
            return;
        }

        float offset = 3;
        if (ai.character.teamID == 1)
            otherPos.x -= offset;
        else if (ai.character.teamID == 2)
            otherPos.x += offset;       

        ai.character.ballCatcher.LookAt(otherPos);
        CharacterActions.kickTypes kickType;

        //tipos de pase
        if (ai.character.teamID == 2 && otherPos.x > 9
          || ai.character.teamID == 1 && otherPos.x < -9
          )
            ai.character.Kick(CharacterActions.kickTypes.CENTRO, (float)Random.Range(1, 15) / 10);
        else
            ai.character.Kick(CharacterActions.kickTypes.SOFT, (float)Random.Range(10, 30) / 10);
        
        SetState(ai.aiPosition);
    }
}
