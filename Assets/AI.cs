using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Character character;
    public states state;

    AIPosition aiPosition;

    public enum states
    {
        ATTACKING,
        DEFENDING,
        NONE
    }
    void Start()
    {
       
        character = GetComponent<Character>();
        Loop();
        Events.CharacterCatchBall += CharacterCatchBall;
        Events.OnBallKicked += OnBallKicked;
        aiPosition = GetComponent<AIPosition>();
    }
    private void OnDestroy()
    {
        Events.CharacterCatchBall -= CharacterCatchBall;
        Events.OnBallKicked -= OnBallKicked;
    }
    void Loop()
    {
        Invoke("Loop", 1);
        CheckState();
    }
    void CheckState()
    {

    }
    void OnBallKicked()
    {
        state = states.NONE;
    }
    void CharacterCatchBall(Character _character)
    {

        if (character.teamID == _character.teamID)
            state = states.ATTACKING;
        else
            state = states.DEFENDING;

        if (character.isBeingControlled)
            ResetAll();
        else
            aiPosition.SetActive(OnPosition);
    }
    void OnPosition()
    {
        ResetAll();
    }
    void ResetAll()
    {
        aiPosition.enabled = false;
    }
}
