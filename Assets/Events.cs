using UnityEngine;
using System.Collections;

public static class Events
{
    public static System.Action ResetApp = delegate { };
    public static System.Action<Game.states> OnGameStatusChanged = delegate { };
    public static System.Action<string> GotoTo = delegate { };
    public static System.Action<string> GotoBackTo = delegate { };
    public static System.Action Back = delegate { };

    public static System.Action<Character> CharacterCatchBall = delegate { };
    public static System.Action OnBallKicked = delegate { };
    public static System.Action<int> OnGoal = delegate { };
    public static System.Action<Character, string> SetDialogue = delegate { };
    public static System.Action<int, InputManagerUI.buttonTypes> OnButtonPressed = delegate { };

    public static System.Action KickToGoal = delegate { };
}
   
