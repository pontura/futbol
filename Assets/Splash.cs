using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    void Start()
    {
        Events.OnButtonClick += OnButtonClick;
    }
    void OnDestroy()
    {
        Events.OnButtonClick -= OnButtonClick;
    }
    void OnButtonClick(int buttonID, int playerID)
    {
        GotoGame();
    }
    public void GotoGame()
    {
        Data.Instance.LoadLevel("Selector");
    }
}
