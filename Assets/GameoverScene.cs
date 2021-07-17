using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameoverScene : MonoBehaviour
{
    public Character character_to_instantiate;
    public GameObject[] allCharacters;
    public CharactersManager charactersManager;
    public List<Character> team;

    void Start()
    {
        Events.OnGoalDone += GoOn;
        Events.OnSkipOn(OnSkip);
        StartCoroutine(On());
    }
    void OnDestroy()
    {
        Events.OnGoalDone -= GoOn;
    }
    int audioID = 0;
    void OnPita()
    {
        audioID++;
        if (audioID >= 3)
            Sigue();
        else
            Events.OnOutroSound(OnPita, audioID);
    }
    void Sigue()
    {
        Events.OnGameOverVoiceHappy(winCharacter, OnSkip);
    }
    Character winCharacter;
    IEnumerator On()
    {
        Events.OnOutroSound(OnPita, audioID);
        int totalCharacters = Data.Instance.matchData.totalCharacters;
        Character character;
        for (int a = 0; a < totalCharacters; a++)
        {
            character = Instantiate(character_to_instantiate, Vector3.zero, Quaternion.identity, allCharacters[a].transform);
            character.transform.localPosition = Vector3.zero;
            

            if (a == 0)
                character.type = Character.types.GOALKEEPER;
            else
                character.type = Character.types.CENTRAL;

        }
        GetComponent<DialoguesManager>().Init();
        Events.ChangeVolume("croud", 0.25f);
        if (Data.Instance.matchData.lastGoalBy == 2)
            charactersManager.Init(true);
        else
            charactersManager.Init(false);


        for (int id = 0; id < totalCharacters; id++)
        {
            character = charactersManager.team1[id];
            character.gameObject.SetActive(true);

            character.transform.localScale = Vector3.one;
            character.actions.Goal();

            if (id == 0)
                winCharacter = character;
        }

       

        yield return new WaitForSeconds(0.2f);
        print("pita");
        charactersManager.referi.actions.Pita();
        SetGoalToCharacter();
        yield return new WaitForSeconds(9);
        Events.ChangeVolume("croud", 0.5f);
    }
    void SetGoalToCharacter()
    {
        int characterGoalID = VoicesManager.Instance.characterGoalID;
        int num = 0;

        Character characterToSwitch = charactersManager.team1[0];
        Character character = charactersManager.team1[1];
        foreach (Character ch in charactersManager.team1)
        {
            if(ch.data.id == characterGoalID)
            {
                if (num == 0)
                    return;
                character = ch;
            }
            num++;
        }
        Transform containerWinner = characterToSwitch.transform.parent.transform;
        Transform containerOther = character.transform.parent.transform;

        characterToSwitch.transform.SetParent(containerOther);
        character.transform.SetParent(containerWinner);

        characterToSwitch.transform.localPosition = Vector3.zero;
        character.transform.localPosition = Vector3.zero;
        character.transform.localScale = Vector3.one;

        print(characterGoalID + "::  characterToSwitch.data.id: " +  characterToSwitch.data.id + " __________________id: " + character.data.id);
    }
    //void SetCharacterOn(Character character)
    //{
    //    Events.SetDialogue(character, Data.Instance.textsData.GetRandomDialogue("goal", character.data.id, character.type == Character.types.GOALKEEPER));
    //}
    void OnSkip()
    {
        Invoke("GoOn", 15);
    }
    void GoOn()
    {
        StopAllCoroutines();
        Data.Instance.LoadLevel("1_MainMenu");
        Events.OnSkipOff();
    }
}
