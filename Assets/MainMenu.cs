using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        Events.PlaySound("crowd", "fulbo_music1", true);
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
