using UnityEngine;
using System.Collections;

public static class Events
{
    public static System.Action ResetApp = delegate { };
    public static System.Action<System.Action> OnSkipOn = delegate { };
    public static System.Action OnSkipOff = delegate { };
    public static System.Action<bool> PlayerProgressBarSetState = delegate { };
    public static System.Action<Game.states> OnGameStatusChanged = delegate { };

    public static System.Action GameInit = delegate { };
    public static System.Action GameOver = delegate { };

    public static System.Action<string> GotoTo = delegate { };
    public static System.Action<string> GotoBackTo = delegate { };
    public static System.Action Back = delegate { };

    public static System.Action<int, int> OnButtonDown= delegate { };
    public static System.Action<int, int> OnButtonClick = delegate { };

    public static System.Action<int, bool> OnRight = delegate { };
    public static System.Action<int, bool> OnUp = delegate { };

    public static System.Action<Character> CharacterCatchBall = delegate { };
    public static System.Action<Character> SetCharacterNewDefender = delegate { };
    public static System.Action<CharacterActions.kickTypes, float, Character> OnBallKicked = delegate { };
    public static System.Action<int, Character> OnGoal = delegate { };
    public static System.Action<Character> SwapCharacter = delegate { };
    public static System.Action<int, Character> OnIntroSound = delegate { };
    public static System.Action<System.Action, int> OnOutroSound = delegate { };
    public static System.Action<Character, System.Action> OnGameOverVoiceHappy = delegate { };
    
    public static System.Action<Character, string> SetDialogue = delegate { };
    public static System.Action<int, InputManagerUI.buttonTypes> OnButtonPressed = delegate { };

    public static System.Action KickToGoal = delegate { };

    public static System.Action<string, string, bool> PlaySound = delegate { };
    public static System.Action<string, float> ChangeVolume = delegate { };
    public static System.Action OnRestartGame = delegate { };
    public static System.Action<Character> OnPenalty = delegate { };
    public static System.Action<Character, System.Action> OnPenaltyWaitingToKick = delegate { };

}
   
