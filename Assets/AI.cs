using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [HideInInspector] public Character character;
    [HideInInspector] public Character characterWithBall;
    public states state;
    [HideInInspector] public Ball ball;

    [HideInInspector] public AIPosition aiPosition;
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

    public enum states
    {
        ATTACKING,
        DEFENDING,
        NONE
    }
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
            aiPosition = new AIPosition();
            aiGotoBall = new AiGotoBall();
            aiHasBall = new AiHasBall();

            aiIdle.Init(this);
            aiPosition.Init(this);
            aiGotoBall.Init(this);
            aiHasBall.Init(this);

            currentState = aiIdle;
        }
        ResetAll();
    }
    public void SetGoalkeeperValues()
    {
        areaEnding_x = Mathf.Abs(originalPosition.x) - Data.Instance.settings.gameplay.gkSpeed_sale_x;
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
        if (state != states.ATTACKING)
            currentState.OnFollow(ball.transform);
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
    void OnBallKicked(CharacterActions.kickTypes kickType, float forceForce, Character character)
    {
        if (character == null) return;

        state = states.NONE;
        ResetAll();
    }
    void SetCharacterNewDefender(Character _character)
    {
        if (character.teamID != _character.teamID)  return;
        if (_character.data.id == character.data.id)
            currentState.OnFollow(ball.transform);
    }
    public virtual void CharacterCatchBall(Character characterWithBall)
    {
        if (Game.Instance.state != Game.states.PLAYING) return;

        if (character.teamID == characterWithBall.teamID)
            state = states.ATTACKING;
        else
            state = states.DEFENDING;

        this.characterWithBall = characterWithBall;
        currentState.OnCharacterCatchBall(characterWithBall);
    }
    public void ResetPosition()
    {
        currentState.ResetAll();
        transform.position = originalPosition;
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

        float sale_z = Data.Instance.settings.gameplay.gkSpeed_sale_z;

        if (dest.z > sale_z) dest.z = sale_z;
        else if (dest.z < -sale_z) dest.z = -sale_z;

        return dest;
    }
}
