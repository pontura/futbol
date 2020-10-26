using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputManager : MonoBehaviour, IInputManager
{

    private static InputManager _instance;

    public static IInputManager instance { get { return _instance; } }

    protected virtual void Awake()
    {
        if(_dontDestroyOnLoad){
            DontDestroyOnLoad (this.transform.root.gameObject);
        }
    }
    public static void SetInstance(InputManager instance)
    {
        if (InputManager._instance == instance)
            return;
        if (InputManager._instance != null){
            InputManager._instance.enabled = false;
        }
        InputManager._instance = instance;
    }

    private bool _dontDestroyOnLoad = true;

    public virtual bool isEnabled { 
        get 
        {
            return this.isActiveAndEnabled;
        } 
        set {
            this.enabled = value;
        } 
    }
    protected virtual void Start() {}
    protected virtual void OnEnable() {}
    protected virtual void OnDisable() {}
    protected virtual void Update() {}
    protected virtual void OnDestroy() {}

    public abstract bool GetButton(int playerID, InputAction inputAction);
    public abstract bool GetButtonDown(int playerID, InputAction inputAction);
    public abstract bool GetButtonUp(int playerID, InputAction inputAction);
    public abstract float GetAxis(int playerID, InputAction inputAction);
}
