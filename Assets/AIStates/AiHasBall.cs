using UnityEngine;

public class AiHasBall : AIState
{
    Vector3 dest;
    float center_goto_goal_x = 12;
    int _z = 0;
    Vector3 limits;
    float timerDestination;
    Vector2 stadiumSize;
    float initTimer;

    public override void Init(AI ai)
    {
        initTimer = Time.time;
        stadiumSize = new Vector2(Data.Instance.stadiumData.active.size_x, Data.Instance.stadiumData.active.size_y);
        limits = new Vector2(stadiumSize.x, stadiumSize.y);
        base.Init(ai);
        color = Color.red;       
    }
    public override void OnCharacterCatchBall(Character character)
    {
        SetState(ai.aiIdle);
    }
    public override void SetActive()
    {        
        timer = 0;
        SetDestination();
       
        if (ai.character.transform.position.z < 0)
            dest.z *= -1;

        if (dest.z > limits.y / 2)
            dest.z = limits.y;
        else if (dest.z < -limits.y / 2)
            dest.z = -limits.y;

    }
    void SetDestination()
    {
        timerDestination = Time.time;
        if (ai.ball.character == null || ai.ball.character != ai.character)
        {
            SetState(ai.aiIdle);
            return;
        }
        center_goto_goal_x = stadiumSize.x/ 2 - Utils.GetRandomFloatBetween(8, 14);
        if (center_goto_goal_x < Mathf.Abs(ai.character.transform.position.x))
            center_goto_goal_x = Mathf.Abs(ai.character.transform.position.x) + 0.25f;
        dest = Vector3.zero;
        dest.x = center_goto_goal_x;
        if (ai.character.teamID == 1)
            dest.x *= -1;
        dest.z = Utils.GetRandomFloatBetween(0, 5);
    }
    public override AIState UpdatedByAI()
    {
        timer += Time.deltaTime;
        if (timer > 0.5f && CheckPase())
        {            
            SetState(ai.aiIdle);
            return State();
        }
        if (Mathf.Abs(ai.transform.position.x+0.25f) > center_goto_goal_x)
            KickBall();
        else
            RunToGoal();

        return State();
    }
    void RunToGoal()
    {
        if (timer == 0)
        {
            ai.character.SuperRun();
            if (Mathf.Abs(ai.transform.position.z - dest.z) < 3) _z = 0;
            else if (Random.Range(0,10)<5)  _z = 1;
            else  _z = -1;
        }

        int _x;
        if (Mathf.Abs(ai.transform.position.x - dest.x)<0.2f)
            _x = 0;
        else if (ai.transform.position.x < dest.x)
            _x = 1;
        else
            _x = -1;

        ai.character.MoveTo(_x, _z);
    }
    void KickBall()
    {
        if (initTimer + 0.25f > Time.time)
        {
            ai.character.Jueguito();
        }
        //else 
        float _z = (stadiumSize.y / 2) * 0.7f;
        Debug.Log(_z);
        if (Mathf.Abs(ai.character.transform.position.z) > _z && Random.Range(0,10)<4)
        {
            Character characterToPass = Game.Instance.charactersManager.GetNearestTo(ai.character, ai.character.teamID);

            float diffX = Mathf.Abs(characterToPass.transform.position.x - ai.transform.position.x);
            if (diffX < 5)
            {
                ai.character.ballCatcher.LookAt(characterToPass.transform.position);
                ai.character.Kick(CharacterActions.kickTypes.CENTRO, Utils.GetRandomFloatBetween(0.7f, 3.5f));
                SetState(ai.aiIdle);
                return;
            }
        }

        KickToGoal();

    }
    void KickToGoal()
    {
        ai.character.Kick(CharacterActions.kickTypes.KICK_TO_GOAL);
        SetState(ai.aiPositionAttacking);
    }
    bool CheckPase()
    {
        timer = 0;
        if (Random.Range(0, 10) < 5) return false;
        Character characterToPass = Game.Instance.charactersManager.GetNearestTo(ai.character, ai.character.teamID);

        if (characterToPass == null) return false;
        Vector3 otherPos = characterToPass.transform.position;

        //fuera de posicion de pase:
        if (ai.character.teamID == 2 && otherPos.x < ai.transform.position.x - 2
            || ai.character.teamID == 1 && otherPos.x > ai.transform.position.x + 2
            ) return false;

        float offset = 3;
        if (ai.character.teamID == 1)
            otherPos.x -= offset;
        else if (ai.character.teamID == 2)
            otherPos.x += offset;       

        ai.character.ballCatcher.LookAt(otherPos);
        CharacterActions.kickTypes kickType;

        float corner_x = (Data.Instance.stadiumData.active.size_x/2)*0.7f;
        //tipos de pase
        if (ai.character.teamID == 2 && otherPos.x > corner_x || ai.character.teamID == 1 && otherPos.x < -corner_x)
            ai.character.Kick(CharacterActions.kickTypes.CENTRO, Utils.GetRandomFloatBetween(0.5f, 2));
        else
            ai.character.Kick(CharacterActions.kickTypes.SOFT, Utils.GetRandomFloatBetween(0.5f,2));

        return true;
    }
}
