using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvas : MonoBehaviour
{
    public GameObject goalAsset;

    void Start()
    {
        Events.OnGoal += OnGoal;
        goalAsset.SetActive(false);
    }

    void OnGoal(int teamID, Character c)
    {
        goalAsset.SetActive(true);
    }
}
