using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [HideInInspector] public Character character;
    [HideInInspector] public Character characterWithBall;
   //    public states state;
    [HideInInspector] public Ball ball;

    [HideInInspector] public AIPositionDefending aiPositionDefending;
    [HideInInspector] public AIPositionAttacking aiPositionAttacking;
    [HideInInspector] public AiGotoBall aiGotoBall;
    [HideInInspector] public AiHasBall aiHasBall;
    [HideInInspector] public AIIdle aiIdle;

    [HideInInspector] public AiIdleGK aiIdleGK;
    [HideInInspector] public AiPositionGK aiPositionGK;
    [HideInInspector] public AIFlyingGK aiFlyingGK;
    [HideInInspector] public AiAlertGK aiAlertGK;
    [HideInInspector] public AIHasBallGK aiHasBallGK;

    public AIState currentState;
    [HideInInspector] public Vector3 originalPosition;

    public string aiStateName;
    public MeshRenderer debugAsset;
    

    float areaEnding_x;

    //public enum states
    //{
    //    ATTACKING,
    //    DEFENDING,
    //    NONE
    //}
    public virtual void Init()
    {
        if (!Data.Instance.DEBUG)
            debugAsset.enabled = false;

        Events.OnGoal += OnGoal;
        Events.CharacterCatchBall += CharacterCatchBall;
        Events.OnBallKicked += OnBallKicked;
        Events.SetCharacterNewDefender += SetCharacterNewDefender;

        originalPosition = transform.position;

        ball = Game.Instance.ball;
        character = GetComponent<Character>();

        if (character.type == Character.types.GOALKEEPER)
        {
            SetGoalkeeperValues();
            aiIdleGK = new AiIdleGK();
            aiPositionGK = new AiPositionGK();
           
            aiFlyingGK = new AIFlyingGK();
            aiAlertGK = new AiAlertGK();
            aiHasBallGK = new AIHasBallGK();

            aiIdleGK.Init(this);
            aiPositionGK.Init(this);
            aiFlyingGK.Init(this);
            aiAlertGK.Init(this);
            aiHasBallGK.Init(this);

            currentState = aiIdleGK;
            aiStateName = currentState.ToString();
        }
        else
        {
            aiIdle = new AIIdle();
            aiPositionDefending = new AIPositionDefending();
            aiPositionAttacking = new AIPositionAttacking();
            aiGotoBall = new AiGotoBall();
            aiHasBall = new AiHasBall();

            aiIdle.Init(this);
            aiPositionDefending.Init(this);
            aiPositionAttacking.Init(this);
            aiGotoBall.Init(this);
            aiHasBall.Init(this);

            currentState = aiIdle;
        }
        ResetAll();
    }
    public void SetGoalkeeperValues()
    {
        areaEnding_x = Mathf.Abs(originalPosition.x) - Data.Instance.settings.gkSpeed_sale_x;
        if (character.teamID == 2)
            areaEnding_x *= -1;

        print(character.teamID + "  " + areaEnding_x);
    }
    private void OnDestroy()
    {
        Events.OnGoal -= OnGoal;
        Events.CharacterCatchBall -= CharacterCatchBall;
        Events.OnBallKicked -= OnBallKicked;
        Events.SetCharacterNewDefender -= SetCharacterNewDefender;
    }
    public void OnBallNearOnAir()
    {
        if (Game.Instance.state != Game.states.PLAYING) return;
        if(character.type == Character.types.GOALKEEPER)
        {
            currentState.OnBallNearOnAir();
            return;
        }            
        if (character.isBeingControlled) return;
        if (
            character.actions.state != CharacterActions.states.IDLE &&
            character.actions.state != CharacterActions.states.RUN
           )
            return;
        currentState.OnBallNearOnAir();
        ResetAll();
        character.Jump();
    }
    public void OnBallNear()
    {
        if (ball.character != null && ball.character.teamID == character.teamID && ball.character.type == Character.types.GOALKEEPER)
            return;

        //la tiene uno del otro equipo y no es el arquero:
        Character characterNearest = character.charactersManager.GetNearest(character.teamID, false, ball.transform.position, true);
        if (characterNearest == character)
        {
            SetStopFollowOthers();
            currentState.OnFollow(ball.transform);
        }
    }
    void SetStopFollowOthers()
    {
        foreach (Character ch in character.charactersManager.GetCharactersByTeam(character.teamID))
            if (ch != character && ch.ai.aiStateName == "AiGotoBall")
                ch.ai.ResetAll(); // si alguien lo seguía chau:
    }
    private void Update()
    {        
        if (Game.Instance.state != Game.states.PLAYING) return;
        if (character.isBeingControlled)   return;

        currentState = currentState.UpdatedByAI();
        aiStateName = currentState.ToString();
    }
    public void SetNewDebugColor(Color color)
    {
        debugAsset.material.color = color;
    }
    void OnGoal(int teamID, Character character)
    {
        ResetAll();
    }
    void OnBallKicked(CharacterActions.kickTypes kickType, float forceForce, Character _character)
    {
        if (_character == null) //fue un cabezaso o algo insignificante...
            return;
        float _z = _character.transform.position.z;
        if (_character.type == Character.types.GOALKEEPER)
        {
            if (character.type == Character.types.CENTRAL)
                currentState.GotoBall();
            return;
        }
        switch (kickType)
        {
            case CharacterActions.kickTypes.CENTRO:
            if (_character.teamID != character.teamID)
            {
                if (character.type == Character.types.DEFENSOR)
                {
                    if ((_z > 0 && character.fieldPosition == Character.fieldPositions.DOWN) || (_z < 0 && character.fieldPosition == Character.fieldPositions.UP))
                        currentState.GotoBall();
                }
            }
            break;
        }
    }
    void SetCharacterNewDefender(Character _character)
    {
        if (character != _character)  return;
        if(ball.character != null && ball.character.type == Character.types.GOALKEEPER) return;
        SetStopFollowOthers();
        currentState.OnFollow(ball.transform);
    }
    public void CharacterCatchBall(Character characterWithBall)
    {
        if (Game.Instance.state != Game.states.PLAYING) return;
        this.characterWithBall = characterWithBall;
        currentState.OnCharacterCatchBall(characterWithBall);
    }
    public void ResetPosition()
    {
        currentState.ResetAll();
       
        int sacaTeamID = Data.Instance.matchData.lastGoalBy;
        if (sacaTeamID == 0) sacaTeamID = 1;
        else if (sacaTeamID == 1) sacaTeamID = 2;
        else if (sacaTeamID == 2) sacaTeamID = 1;
        if (character.teamID == sacaTeamID && character.type == Character.types.DELANTERO && character.fieldPosition == Character.fieldPositions.UP)
            SetInitialCharacterPosition(sacaTeamID);
        else transform.position = originalPosition;
    }
    void SetInitialCharacterPosition(int sacaTeamID)
    {
        int _x = 1;
        if (sacaTeamID == 2)
            _x *= -1;
        transform.position = new Vector3(_x, originalPosition.y, 0);
    }
    public void ResetAll()
    {
        currentState.ResetAll();
    }

    public Vector3 GetPositionInsideArea(Vector3 to)
    {
        Vector3 dest = to;

        if (character.teamID == 1 && areaEnding_x > to.x)
            dest.x = areaEnding_x;
        else if (character.teamID == 2 && areaEnding_x < to.x)
            dest.x = areaEnding_x;

        float sale_z = Data.Instance.settings.gkSpeed_sale_z;

        if (dest.z > sale_z) dest.z = sale_z;
        else if (dest.z < -sale_z) dest.z = -sale_z;

        return dest;
    }
}
