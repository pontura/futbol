using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class MainMenu : MonoBehaviour
{
    public Rewired.UI.ControlMapper.ControlMapper controlMapper_to_add;
    Rewired.UI.ControlMapper.ControlMapper controlMapper;
    public Canvas controlMappingCanvas;
    public GameObject credits;
    public GameObject controls;

    void Start()
    {
        SetControls(false);
        SetCredits(false);
        Events.PlaySound("crowd", "", true);

        if(!Data.Instance.isMobile)
            Events.OnButtonClick += OnButtonClick;
        //controlMapper = Instantiate( controlMapper_to_add);
        //controlMapper.rewiredInputManager = GameObject.Find("Rewired Input Manager(Clone)").GetComponent< Rewired.InputManager>();
        //controlMappingCanvas = controlMapper.GetComponentInChildren<Canvas>();
    }
    void OnDestroy()
    {
        Events.OnButtonClick -= OnButtonClick;
    }
    void OnButtonClick(int buttonID, int playerID)
    {

        if (controlMapper != null && controlMapper.isOpen)
            return;
        GotoGame();

    }
    public void GotoGame()
    {
        Data.Instance.stadiumData.SetActiveStadium(Data.Instance.stadiumData.id);
        Data.Instance.LoadLevel("Selector");

        //GetComponent<DifficultSelector>().Init();
    }
    public void Controls()
    {
        if (controlMapper == null)
            return;
        controlMapper.Open();
        SetControls(false);
    }
    public void SetCredits(bool isOn)
    {
        credits.SetActive(isOn);
    }
    public void SetControls(bool isOn)
    {
        controls.SetActive(isOn);
    }
    public void closeControls(bool isOn)
    {
        credits.SetActive(isOn);
    }
    public void gotoParsec()
    {
        Data.Instance.LoadLevel("Parsec");
    }
}
