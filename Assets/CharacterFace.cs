using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFace : MonoBehaviour
{
    public GameObject idle;
    public GameObject angry;
    Character character;

    private void Start()
    {
        character = GetComponentInParent<Character>();
        Idle();
        Events.OnGoal += OnGoal;
        Events.OnGameStatusChanged += OnGameStatusChanged;
    }
    private void OnDestroy()
    {
        Events.OnGameStatusChanged -= OnGameStatusChanged;
        Events.OnGoal -= OnGoal;
    }
    void OnGameStatusChanged(Game.states state)
    {
        if (state == Game.states.PLAYING)
            Idle();
    }
    void OnGoal(int teamID)
    {
        if (character.teamID == teamID)
            Idle();
        else
            Angry();
    }
    private void Reset()
    {
        idle.SetActive(false);
        angry.SetActive(false);
    }
    public void Idle()
    {
        Reset();
        idle.SetActive(true);
    }

    void Angry()
    {
        Reset();
        angry.SetActive(true);
    }
}
