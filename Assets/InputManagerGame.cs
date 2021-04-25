using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeoLuz.PlugAndPlayJoystick;
using System;
public class InputManagerGame : MonoBehaviour
{
    int totalPlayersAvailable;
    float time_to_palancazo = 0.25f;
    public CharactersManager charactersManager;
    public GameObject joystickAsset;
    public AnalogicKnob analogicKnob;
    float input_x_sensibilitty;

    public List<PlayerInput> playerInputs;
    [Serializable] public class PlayerInput
    {
        public int lastDirection;
        public float lastDirectionTime;
        public int totaldirectionChanges;
        public int lastButtonIDPressed;
    }

    private void Start()
    {
        totalPlayersAvailable = Data.Instance.settings.totalPlayersAvailable;

        for (int a = 0; a < 4; a++)
            playerInputs.Add(new PlayerInput());

        if (Data.Instance.isMobile)
            joystickAsset.SetActive(true);
        else
            joystickAsset.SetActive(false);
        input_x_sensibilitty = Data.Instance.settings.input_x_sensibilitty;

        Events.CharacterCatchBall += CharacterCatchBall;
    }
    private void OnDestroy()
    {
        Events.CharacterCatchBall -= CharacterCatchBall;
    }
    void CharacterCatchBall(Character ch)
    {
    }
    void Update()
    {
        if (Game.Instance.state != Game.states.PLAYING)
            return;

        if(Data.Instance.settings.mainSettings.isArcade)
        {
            for (int id = 0; id < 4; id++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                    UIMain.Instance.GetComponent<IngameMenu>().Open();

                float _x = Input.GetAxis("Horizontal" + (id + 1)) * input_x_sensibilitty;
                float _y = Input.GetAxis("Vertical" + (id + 1));

                if (_x > 1) _x = 1;
                else if (_x < -1) _x = -1;

                if (charactersManager != null)
                    charactersManager.SetPosition(id + 1, _x, _y);
                if (_y != 0)
                    SetNewInput_x(id, _y);


                if (Input.GetButtonDown("Button" + (id + 1) + "_1"))
                    GetButtonDown(3, (id + 1));

                else if (Input.GetButtonDown("Button" + (id + 1) + "_2"))
                    GetButtonDown(2, (id + 1));

                else if (Input.GetButtonDown("Button" + (id + 1) + "_3"))
                    GetButtonDown(1, (id + 1));

                if (Input.GetButtonUp("Button" + (id + 1) + "_1"))
                    GetButtonUp(3, (id + 1));

                else if (Input.GetButtonUp("Button" + (id + 1) + "_2"))
                    GetButtonUp(2, (id + 1));

                else if (Input.GetButtonUp("Button" + (id + 1) + "_3"))
                    GetButtonUp(1, (id + 1));

            }
        } else if (Data.Instance.isMobile)
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
            for (int a = 0; a < totalPlayersAvailable; a++)
            {
                float _x = InputManager.instance.GetAxis(a, InputAction.horizontal) * input_x_sensibilitty;
                float _y = InputManager.instance.GetAxis(a, InputAction.vertical);
                if (_x > 1) _x = 1;
                else if (_x < -1) _x = -1;

                if (charactersManager != null)
                    charactersManager.SetPosition(a+1, _x, _y);
                if (_y != 0)
                    SetNewInput_x(a, _y);
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
        }
    }
    public void OnMobileButtonPressed(int id)
    {
        GetButtonDown(id, 1);
    }
    public void OnMobileButtonUp(int id)
    {
        GetButtonUp(id, 1);
    }
    void GetButtonDown(int buttonID, int playerID)
    {

        int lastButtonPressedID = GetInputByPlayer(playerID-1).lastButtonIDPressed;

        if ((lastButtonPressedID == 2 && buttonID == 1) || (lastButtonPressedID == 1 && buttonID == 2))
        {
            GetInputByPlayer(playerID - 1).lastButtonIDPressed = 0;
            charactersManager.Palancazo(playerID);
        }            
        else if (charactersManager != null)
            charactersManager.ButtonPressed(buttonID, playerID);
        else
            Events.OnButtonClick(buttonID, playerID);

        GetInputByPlayer(playerID-1).lastButtonIDPressed = buttonID;
    }
    void GetButtonUp(int buttonID, int playerID)
    {
        if (GetInputByPlayer(playerID - 1).lastButtonIDPressed == 0) return;
        GetInputByPlayer(playerID-1).lastButtonIDPressed = 0;

        if (charactersManager != null)
            charactersManager.ButtonUp(buttonID, playerID);
        else
            Events.OnButtonClick(buttonID, playerID);
    }
    void AddPlayer(int playerID, int teamID)
    {
        Game.Instance.charactersManager.AddCharacter(playerID, teamID);
    }
    void SetNewInput_x(int playerID, float _x)
    {
        PlayerInput playerInput = GetInputByPlayer(playerID);
        int dir = (int)Mathf.Ceil(_x);
        if (dir != 0 && playerInput.lastDirection != dir)
        {
            //if (playerInput.lastDirectionTime == 0 || playerInput.lastDirectionTime + time_to_palancazo > Time.time)
            //{
            //    charactersManager.Palancazo(playerID+1);
            //    playerInput.totaldirectionChanges = 0;
            //}
            playerInput.lastDirectionTime = Time.time;
        }
        if (playerInput.lastDirectionTime + time_to_palancazo < Time.time)
        {
            playerInput.totaldirectionChanges = 0;
        }            
        playerInput.lastDirection = dir;
    }
    PlayerInput GetInputByPlayer(int playerID)
    {
        print("playerID " + playerID);
        return playerInputs[playerID];
    }
}
