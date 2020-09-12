using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Character character;
    public states state;
    public Ball ball;
    public AIPosition aiPosition;
    public AiGotoBall aiGotoBall;

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
        Events.CharacterCatchBall += CharacterCatchBall;
        Events.OnBallKicked += OnBallKicked;
        Events.SetCharacterNewDefender += SetCharacterNewDefender;
        aiPosition = GetComponent<AIPosition>();
        aiGotoBall = GetComponent<AiGotoBall>();
    }
    private void OnDestroy()
    {
        Events.CharacterCatchBall -= CharacterCatchBall;
        Events.OnBallKicked -= OnBallKicked;
        Events.SetCharacterNewDefender -= SetCharacterNewDefender;
    }
    private void Update()
    {
        if (character.isBeingControlled)
            return;
        else if (aiGotoBall.enabled)
            aiGotoBall.UpdatedByAI();
        else if(aiPosition.enabled)
            aiPosition.UpdatedByAI();
    }
    void OnBallKicked()
    {
        state = states.NONE;
    }
    void SetCharacterNewDefender(Character _character)
    {
        if (character.teamID != _character.teamID || aiGotoBall.enabled == true)
            return;
        if (_character.characterID == character.characterID)
        {
            ResetAll();
            aiGotoBall.SetActive();
        } 
        else
            aiGotoBall.Reset();
    }
    public virtual void CharacterCatchBall(Character _character)
    {
        aiGotoBall.Reset();
        if (character.teamID == _character.teamID)
            state = states.ATTACKING;
        else
            state = states.DEFENDING;

        if (character.isBeingControlled)
            ResetAll();
        else
            aiPosition.SetActive();
    }
    public void ResetPosition()
    {
        ResetAll();
        aiPosition.ResetPosition();
        aiGotoBall.Reset();
    }
    public void ResetAll()
    {
        aiPosition.enabled = false;
        aiGotoBall.enabled = false;
    }
}
