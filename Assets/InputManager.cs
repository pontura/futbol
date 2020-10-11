﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeoLuz.PlugAndPlayJoystick;

public class InputManager : MonoBehaviour
{
    public CharactersManager charactersManager;
   
    public GameObject joystickAsset;
    public AnalogicKnob analogicKnob;

    private void Start()
    {
        if (Data.Instance.isMobile)
            joystickAsset.SetActive(true);
        else
            joystickAsset.SetActive(false);
    }

    void Update()
    {
        if (Game.Instance.state != Game.states.PLAYING)
            return;

        if (Data.Instance.isMobile)
        {
            float _x = analogicKnob.NormalizedAxis.x;
            float _y = analogicKnob.NormalizedAxis.y;
            float offset = 0.15f;
            if (_x > offset) _x = 1; else if (_x < -offset) _x = -1;
            if (_y > offset) _y= 1; else if (_y < -offset) _y = -1;
            if (charactersManager != null)
                charactersManager.SetPosition(1, _x, _y);
        }
        else
        {

            if (Input.GetKeyDown(KeyCode.Alpha1)) Events.KickToGoal();

            //if (Input.GetButtonDown("Button1_1") && !charactersManager.player1) charactersManager.AddCharacter(1);
            //if (Input.GetButtonDown("Button1_2") && !charactersManager.player2) charactersManager.AddCharacter(2);
           // if (Input.GetButtonDown("Button1_3") && !charactersManager.player3) charactersManager.AddCharacter(3);
           // if (Input.GetButtonDown("Button1_4") && !charactersManager.player4) charactersManager.AddCharacter(4);

            for (int id = 1; id < charactersManager.totalPlayers + 1; id++)
            {
                float _x = Input.GetAxis("Horizontal" + id);
                float _y = Input.GetAxis("Vertical" + id);

                if (charactersManager != null)
                    charactersManager.SetPosition(id, _x, _y);

                if (Input.GetButtonDown("Button1_" + id))
                    ButtonPressed(1, id);

                else if (Input.GetButtonDown("Button2_" + id))
                    ButtonPressed(2, id);

                else if (Input.GetButtonDown("Button3_" + id))
                    ButtonPressed(3, id);

            }
        }
    }
    public void OnMobileButtonPressed(int id)
    {
        if (id == 1)
            ButtonPressed(1, 1);
        else if (id == 2)
            ButtonPressed(2, 1);
        else
            ButtonPressed(3, 1);
    }
    void ButtonPressed(int buttonID, int playerID)
    {
        if (charactersManager != null)
            charactersManager.ButtonPressed(buttonID, playerID);
        else
            Events.OnButtonClick(buttonID, playerID);
    }
}
