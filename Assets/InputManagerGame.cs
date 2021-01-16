using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LeoLuz.PlugAndPlayJoystick;
using System;
public class InputManagerGame : MonoBehaviour
{
    float time_to_palancazo = 0.15f;
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
    }

    private void Start()
    {
        for (int a = 0; a < 2; a++)
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
        lastButtonDown_p1 = 0;
        lastButtonDown_p2 = 0;
    }
    void Update()
    {
        if (Game.Instance.state != Game.states.PLAYING)
            return;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(Time.timeScale == 0)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;
        }

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
                if (_x != 0)
                    SetNewInput_x(a, _x);
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
        {
            if(Data.Instance.settings.totalPlayers == 1)
            {
                AddPlayer();
                return;
            }
            lastButtonDown_p2 = buttonID;
        }
            

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
    void AddPlayer()
    {
        Data.Instance.settings.totalPlayers++;
        Game.Instance.charactersManager.AddCharacter(2);
    }
    void SetNewInput_x(int playerID, float _x)
    {
        PlayerInput playerInput = GetInputByPlayer(playerID);
        int dir = (int)Mathf.Ceil(_x);
        if (dir != 0 && playerInput.lastDirection != dir)
        {
            if (playerInput.lastDirectionTime == 0 || playerInput.lastDirectionTime + time_to_palancazo > Time.time)
            {
                charactersManager.Palancazo(playerID+1);
                playerInput.totaldirectionChanges = 0;
            }
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
        return playerInputs[playerID];
    }
}
