using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalAsset : MonoBehaviour
{
    public GameObject goalAsset;
    public GameObject notGoalAsset;
    public GameObject[] teams;

    void Start()
    {
        Loop();
        Events.OnGoal += OnGoal;
    }
    void Loop()
    {
        notGoalAsset.SetActive(true);
        goalAsset.SetActive(false);
    }
    void OnDestroy()
    {
        Events.OnGoal -= OnGoal;
    }
    void OnGoal(int teamID, Character character)
    {
        notGoalAsset.SetActive(false);
        goalAsset.SetActive(true);

        foreach(GameObject go in teams)
            go.SetActive(false);

        teams[character.teamID-1].SetActive(true);

        Invoke("Loop", 5);
    }
    
}
