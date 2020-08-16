using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public CharactersManager charactersManager;

    private void Start()
    {
        charactersManager.AddCharacter(1);
        charactersManager.AddCharacter(2);
    }
    void Update()
    {
        
        //if (Input.GetButtonDown("Button1_1") && !charactersManager.player1) charactersManager.AddCharacter(1);
        //if (Input.GetButtonDown("Button1_2") && !charactersManager.player2) charactersManager.AddCharacter(2);
        //if (Input.GetButtonDown("Kick3") && !charactersManager.player3) charactersManager.AddCharacter(3);
        //if (Input.GetButtonDown("Kick4") && !charactersManager.player4) charactersManager.AddCharacter(4);

        for (int id = 1; id < charactersManager.totalPlayers + 1; id++)
        {
            float _x = Input.GetAxis("Horizontal" + id);
            float _y = Input.GetAxis("Vertical" + id);
            charactersManager.SetPosition(id, _x, _y);

            if (Input.GetButtonDown("Button1_" + id))
                charactersManager.ButtonPressed(1, id);

            if (Input.GetButtonDown("Button2_" + id))
                charactersManager.ButtonPressed(2, id);

            if (Input.GetButtonDown("Button3_" + id))
                charactersManager.ButtonPressed(3, id);
            
        }
        

    }
}
