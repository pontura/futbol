using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeoLuz.PlugAndPlayJoystick;

public class InputManagerGame : MonoBehaviour
{
    public CharactersManager charactersManager;
    public GameObject joystickAsset;
    public AnalogicKnob analogicKnob;
    float input_x_sensibilitty;

    private void Start()
    {
        if (Data.Instance.isMobile)
            joystickAsset.SetActive(true);
        else
            joystickAsset.SetActive(false);
        input_x_sensibilitty = Data.Instance.settings.gameplay.input_x_sensibilitty;

        Events.CharacterCatchBall += CharacterCatchBall;
    }
    private void OnDestroy()
    {
        Events.CharacterCatchBall -= CharacterCatchBall;
    }
    void CharacterCatchBall(Character ch)
    {
        lastButtonDown_p1 = 0;
        lastButtonDown_p2 = 0;
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
            for (int a = 0; a < Data.Instance.settings.totalPlayers; a++)
            {
                float _x = InputManager.instance.GetAxis(a, InputAction.horizontal) * input_x_sensibilitty;
                float _y = InputManager.instance.GetAxis(a, InputAction.vertical);
                if (_x > 1) _x = 1;
                else if (_x < -1) _x = -1;

                if (charactersManager != null)
                    charactersManager.SetPosition(a+1, _x, _y);
            }
          

            for (int a = 0; a < 2; a++)
            {
                if (InputManager.instance.GetButtonDown(a, InputAction.action1))
                    GetButtonDown(1, a+1);
                else if (InputManager.instance.GetButtonUp(a, InputAction.action1))
                    GetButtonUp(1, a + 1);
                else if (InputManager.instance.GetButtonDown(a, InputAction.action2))
                    GetButtonDown(2, a + 1);
                else if (InputManager.instance.GetButtonUp(a, InputAction.action2))
                    GetButtonUp(2, a + 1);
                else if (InputManager.instance.GetButtonDown(a, InputAction.action3))
                    GetButtonDown(3, a + 1);
                else if (InputManager.instance.GetButtonUp(a, InputAction.action3))
                    GetButtonUp(3, a + 1);
            }



            // if (Input.GetKeyDown(KeyCode.Alpha1)) Events.KickToGoal();

            // //if (Input.GetButtonDown("Button1_1") && !charactersManager.player1) charactersManager.AddCharacter(1);
            // //if (Input.GetButtonDown("Button1_2") && !charactersManager.player2) charactersManager.AddCharacter(2);
            //// if (Input.GetButtonDown("Button1_3") && !charactersManager.player3) charactersManager.AddCharacter(3);
            //// if (Input.GetButtonDown("Button1_4") && !charactersManager.player4) charactersManager.AddCharacter(4);

            // for (int id = 1; id < charactersManager.totalPlayers + 1; id++)
            // {
            //     float _x = Input.GetAxis("Horizontal" + id);
            //     float _y = Input.GetAxis("Vertical" + id);

            //     if (charactersManager != null)
            //         charactersManager.SetPosition(id, _x, _y);

            //     if (Input.GetButtonDown("Button1_" + id))
            //         ButtonPressed(1, id);

            //     else if (Input.GetButtonDown("Button2_" + id))
            //         ButtonPressed(2, id);

            //     else if (Input.GetButtonDown("Button3_" + id))
            //         ButtonPressed(3, id);

            // }
        }
    }
    public void OnMobileButtonPressed(int id)
    {
            if (id == 1)
                GetButtonDown(1, 1);
            else if (id == 2)
                GetButtonDown(2, 1);
            else
                GetButtonDown(3, 1);
    }
    public void OnMobileButtonUp(int id, bool isDown)
    {
        if (id == 1)
            GetButtonUp(1, 1);
        else if (id == 2)
            GetButtonUp(2, 1);
        else
            GetButtonUp(3, 1);
    }
    int lastButtonDown_p1;
    int lastButtonDown_p2;
    void GetButtonDown(int buttonID, int playerID)
    {
        if (playerID == 1)
            lastButtonDown_p1 = buttonID;
        else
            lastButtonDown_p2 = buttonID;

        if (charactersManager != null)
            charactersManager.ButtonPressed(buttonID, playerID);
        else
            Events.OnButtonClick(buttonID, playerID);
    }
    void GetButtonUp(int buttonID, int playerID)
    {
        if (playerID == 1 && lastButtonDown_p1 != buttonID
            || playerID == 2 && lastButtonDown_p2 != buttonID)
            return;
        if (charactersManager != null)
            charactersManager.ButtonUp(buttonID, playerID);
        else
            Events.OnButtonClick(buttonID, playerID);
    }
}
