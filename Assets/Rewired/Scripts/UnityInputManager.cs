using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityInputManager : InputManager
{
    [SerializeField]
    public string _playerAxisPref = "";
    [SerializeField]
    public int _maxNumberOfPlayers = 1;
    [SerializeField]
    private string _action1 = "Fire1";
    [SerializeField]
    private string _action2 = "Fire2";
    [SerializeField]
    private string _action3 = "Fire3";
    [SerializeField]
    private string _pauseAxis = "Cancel";

    private Dictionary<int, string>[] _actions;

    protected override void Awake()
    {
        base.Awake();
        if (InputManager.instance != null)
        {
            isEnabled = false;
            return;
        }
        SetInstance(this);

        _actions = new Dictionary<int, string>[_maxNumberOfPlayers];

        for (int i = 0; i < _maxNumberOfPlayers; i++)
        {
            Dictionary<int, string> playerActions = new Dictionary<int, string>();
            _actions[i] = playerActions;
            string prefix = !string.IsNullOrEmpty(_playerAxisPref) ? _playerAxisPref + i : string.Empty;
            AddAction(InputAction.action1, prefix + _action1, playerActions);
            AddAction(InputAction.action2, prefix + _action1, playerActions);
            AddAction(InputAction.action3, prefix + _action1, playerActions);
        }
    }

    private static void AddAction(InputAction action, string actionName, Dictionary<int, string> _actions)
    {
        if (string.IsNullOrEmpty(actionName))
        {
            return;
        }
        _actions.Add((int)action, actionName);
    }
    public override bool GetButton(int playerID, InputAction action)
    {
        bool value = Input.GetButton(_actions[playerID][(int)action]);
        //es como Input.GetButton("Fire1"); entendes??
        return value;
    }
    public override bool GetButtonDown(int playerID, InputAction action)
    {
        bool value = Input.GetButtonDown(_actions[playerID][(int)action]);
        //es como Input.GetButton("Fire1"); entendes??
        return value;
    }
    public override bool GetButtonUp(int playerID, InputAction action)
    {
        bool value = Input.GetButtonUp(_actions[playerID][(int)action]);
        //es como Input.GetButton("Fire1"); entendes??
        return value;
    }
    public override float GetAxis(int playerID, InputAction action)
    {
        float value = Input.GetAxisRaw(_actions[playerID][(int)action]);
        //es como Input.GetButton("Fire1"); entendes??
        return value;
    }
}