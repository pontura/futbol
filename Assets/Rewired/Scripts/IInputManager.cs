public interface IInputManager
{
    bool isEnabled { get; set; }

    bool GetButton(int playerID, InputAction inputAction);
    bool GetButtonDown(int playerID, InputAction inputAction);
    bool GetButtonUp(int playerID, InputAction inputAction);

    float GetAxis(int playerID, InputAction inputAction);

}
