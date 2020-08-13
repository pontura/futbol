using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public CharactersManager charactersManager;


    void Update()
    {
        
        if (Input.GetButtonDown("Kick1") && !charactersManager.player1) charactersManager.AddCharacter(1);
        if (Input.GetButtonDown("Kick2") && !charactersManager.player2) charactersManager.AddCharacter(2);
        //if (Input.GetButtonDown("Kick3") && !charactersManager.player3) charactersManager.AddCharacter(3);
        //if (Input.GetButtonDown("Kick4") && !charactersManager.player4) charactersManager.AddCharacter(4);

        for (int id = 1; id < charactersManager.totalPlayers + 1; id++)
        {
            float _x = Input.GetAxis("Horizontal" + id);
            float _y = Input.GetAxis("Vertical" + id);
            charactersManager.SetPosition(id, _x, _y);

            if (Input.GetButtonDown("Kick" + id))
                charactersManager.Kick(id);

            if (Input.GetButtonDown("Special" + id))
                charactersManager.KickAllTheOthers(id);
            
        }
        

    }
}
