using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerUI : MonoBehaviour
{
    public float horizontalAxis_team_1;
    public float horizontalAxis_team_2;

    public float verticalAxis_team_1;
    public float verticalAxis_team_2;


    public enum buttonTypes
    {
        BUTTON_1,
        BUTTON_2,
        BUTTON_3
    }
    void Update()
    {
        if (Data.Instance.isMobile)
        {

            //if (Input.GetButtonDown("Button1_1") && !charactersManager.player1) charactersManager.AddCharacter(1);
            //if (Input.GetButtonDown("Button1_2") && !charactersManager.player2) charactersManager.AddCharacter(2);
            //if (Input.GetButtonDown("Button1_3") && !charactersManager.player3) charactersManager.AddCharacter(3);
            //if (Input.GetButtonDown("Button1_4") && !charactersManager.player4) charactersManager.AddCharacter(4);

            for (int id = 1; id < 3; id++)
            {
                if (id == 1)
                {
                    horizontalAxis_team_1 = Input.GetAxis("Horizontal" + id);
                    verticalAxis_team_1 = Input.GetAxis("Vertical" + id);
                }
                else
                {
                    horizontalAxis_team_2 = Input.GetAxis("Horizontal" + id);
                    verticalAxis_team_2 = Input.GetAxis("Vertical" + id);
                }

                if (Input.GetButtonDown("Button1_" + id))
                    Events.OnButtonPressed(id, buttonTypes.BUTTON_1);

                else if (Input.GetButtonDown("Button2_" + id))
                    Events.OnButtonPressed(id, buttonTypes.BUTTON_2);

                else if (Input.GetButtonDown("Button3_" + id))
                    Events.OnButtonPressed(id, buttonTypes.BUTTON_3);

            }
        }
        else
        {
            int id = 0;
            for (id = 0; id < 3; id++)
            {
                if (InputManager.instance.GetButtonUp(id, InputAction.action1))
                    Events.OnButtonClick(id+1,1);
                if (InputManager.instance.GetButtonUp(id, InputAction.action2))
                    Events.OnButtonClick(id + 1, 2);
                if (InputManager.instance.GetButtonUp(id, InputAction.action3))
                    Events.OnButtonClick(id + 1, 3);
            }
            horizontalAxis_team_1 = InputManager.instance.GetAxis(0, InputAction.horizontal);
            horizontalAxis_team_2 = InputManager.instance.GetAxis(1, InputAction.horizontal);
            verticalAxis_team_1 = InputManager.instance.GetAxis(0, InputAction.vertical);
            verticalAxis_team_2 = InputManager.instance.GetAxis(1, InputAction.vertical);
        }


    }
}
