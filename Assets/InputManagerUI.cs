using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerUI : MonoBehaviour
{
    public enum buttonTypes
    {
        BUTTON_1,
        BUTTON_2,
        BUTTON_3
    }
    void Update()
    {
        //if (Input.GetButtonDown("Button1_1") && !charactersManager.player1) charactersManager.AddCharacter(1);
        //if (Input.GetButtonDown("Button1_2") && !charactersManager.player2) charactersManager.AddCharacter(2);
        //if (Input.GetButtonDown("Button1_3") && !charactersManager.player3) charactersManager.AddCharacter(3);
        //if (Input.GetButtonDown("Button1_4") && !charactersManager.player4) charactersManager.AddCharacter(4);

        for (int id = 1; id < 4; id++)
        {
            //float _x = Input.GetAxis("Horizontal" + id);
            //float _y = Input.GetAxis("Vertical" + id);
            //charactersManager.SetPosition(id, _x, _y);

            if (Input.GetButtonDown("Button1_" + id))
                Events.OnButtonPressed(id, buttonTypes.BUTTON_1);

            else if (Input.GetButtonDown("Button2_" + id))
                Events.OnButtonPressed(id, buttonTypes.BUTTON_2);

            else if (Input.GetButtonDown("Button3_" + id))
                Events.OnButtonPressed(id, buttonTypes.BUTTON_3);

        }


    }
}
