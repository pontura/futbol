using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizablePart : MonoBehaviour
{
    bool setted;

    void Start()
    {
        if (setted)
            return;
        Character character = GetComponentInParent<Character>();
        string[] arr = gameObject.name.Split("_"[0]);
        string colorName = arr[arr.Length - 1];
        Settings.TeamSettings teamSettings = Data.Instance.settings.teamSettings[character.teamID - 1];
        switch(colorName)
        {
            case "A":  GetComponent<SpriteRenderer>().color = teamSettings.clothColorA; break;
            case "B": GetComponent<SpriteRenderer>().color = teamSettings.clothColorB; break;
            case "C": GetComponent<SpriteRenderer>().color = teamSettings.clothColorC; break;
            case "D": GetComponent<SpriteRenderer>().color = teamSettings.clothColorD; break;
        }
        
        setted = true;
    }

}
