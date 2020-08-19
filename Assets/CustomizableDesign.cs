using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizableDesign : MonoBehaviour
{
    [SerializeField] private GameObject[] designs;
    bool setted;
    void Start()
    {
        if (setted) return;
        setted = true;
        Character character = GetComponentInParent<Character>();
        foreach (GameObject go in designs)
            go.SetActive(false);
        Settings.TeamSettings teamSettings = Data.Instance.settings.teamSettings[character.teamID - 1];
        if(teamSettings.designID>0)
            designs[teamSettings.designID-1].SetActive(true);

    }

}
