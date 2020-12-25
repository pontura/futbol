using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizablePart : MonoBehaviour
{
    bool setted;
    public int teamID;

    void Start()
    {
        if (setted)
            return;
        Character character = GetComponentInParent<Character>();

        if (character)
            teamID = character.teamID;

        string[] arr = gameObject.name.Split("_"[0]);
        string colorName = arr[arr.Length - 1];
        Settings.TeamSettings teamSettings = Data.Instance.settings.GetTeamSettings(teamID);
        switch (colorName)
        {
            case "A":  GetComponent<SpriteRenderer>().color = teamSettings.clothColorA; break;
            case "B": GetComponent<SpriteRenderer>().color = teamSettings.clothColorB; break;
            case "C": GetComponent<SpriteRenderer>().color = teamSettings.clothColorC; break;
            case "D": GetComponent<SpriteRenderer>().color = teamSettings.clothColorD; break;
        }
        
        setted = true;
    }

}
