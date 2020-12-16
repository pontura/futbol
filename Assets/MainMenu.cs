using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MainMenu : MonoBehaviour
{
    public Rewired.UI.ControlMapper.ControlMapper controlMapper_to_add;
    Rewired.UI.ControlMapper.ControlMapper controlMapper;
    public Canvas controlMappingCanvas;

    void Start()
    {
        Events.PlaySound("crowd", "fulbo_music1", true);
        Events.OnButtonClick += OnButtonClick;
        controlMapper = Instantiate( controlMapper_to_add);
        controlMapper.rewiredInputManager = GameObject.Find("Rewired Input Manager(Clone)").GetComponent< Rewired.InputManager>();
        controlMappingCanvas = controlMapper.GetComponentInChildren<Canvas>();
    }
    void OnDestroy()
    {
        Events.OnButtonClick -= OnButtonClick;
    }
    void OnButtonClick(int buttonID, int playerID)
    {
        if (controlMapper.isOpen)
            return;
        GotoGame();
    }
    public void GotoGame()
    {
        Data.Instance.LoadLevel("Selector");
    }
    public void Controls()
    {
        controlMapper.Open();
    }
}
