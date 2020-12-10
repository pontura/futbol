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

    public enum states
    {
        ATTACKING,
        DEFENDING,
        NONE
    }
    void Start()
    {
        ball = Game.Instance.ball;
        character = GetComponent<Character>();

        Events.OnGoal += OnGoal;
        Events.CharacterCatchBall += CharacterCatchBall;
        Events.OnBallKicked += OnBallKicked;
        Events.SetCharacterNewDefender += SetCharacterNewDefender;

        aiPosition = GetComponent<AIPosition>();
        aiGotoBall = GetComponent<AiGotoBall>();
        aiHasBall = GetComponent<AiHasBall>();
        ResetAll();
    }
    private void OnDestroy()
    {
        Events.OnGoal -= OnGoal;
        Events.CharacterCatchBall -= CharacterCatchBall;
        Events.OnBallKicked -= OnBallKicked;
        Events.SetCharacterNewDefender -= SetCharacterNewDefender;
    }
    private void Update()
    {
        if (Game.Instance.state != Game.states.PLAYING) return;
        if (character.isBeingControlled)
            return;
        if (aiHasBall != null && aiHasBall.enabled)
            aiHasBall.UpdatedByAI();
        else if (aiGotoBall.enabled)
            aiGotoBall.UpdatedByAI();
        else if (aiPosition.enabled)
            aiPosition.UpdatedByAI();
    }
    void OnGoal(int teamID, Character character)
    {
        ResetAll();
    }
    void OnBallKicked(CharacterActions.kickTypes kickType, float forceForce, Character character)
    {
        state = states.NONE;
    }
    void SetCharacterNewDefender(Character _character)
    {
        if (character.teamID != _character.teamID)
            return;
        if (_character.characterID == character.characterID)
        {
            ResetAll();
            aiGotoBall.SetActive();
        }
        else if (aiGotoBall.enabled)
        {
            aiPosition.SetActive();
            aiGotoBall.Reset();
        }
    }
    public virtual void CharacterCatchBall(Character characterWithBall)
    {
        if (Game.Instance.state != Game.states.PLAYING) return;
        this.characterWithBall = characterWithBall;
        if (character.teamID == characterWithBall.teamID)
            state = states.ATTACKING;
        else
            state = states.DEFENDING;

        if (characterWithBall.characterID == character.characterID)
        {
            if (aiHasBall != null)
            {
                ResetAll();
                aiHasBall.SetActive();
                return;
            }
        }


        if (character.isBeingControlled)
            ResetAll();
        else
        {
            aiGotoBall.Reset();
            if(aiHasBall != null) aiHasBall.Reset();
            aiPosition.SetActive();
        }
    }
    public void ResetPosition()
    {
        ResetAll();
        aiPosition.ResetPosition();
        aiGotoBall.Reset();
        if (aiHasBall != null)
            aiHasBall.Reset();
    }
    public void ResetAll()
    {
        aiGotoBall.Reset();
        aiPosition.Reset();

        aiPosition.enabled = false;
        aiGotoBall.enabled = false;

        if (aiHasBall != null)
        {
            aiHasBall.Reset();
            aiHasBall.enabled = false;
        }
    }
}
