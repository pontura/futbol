using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScene : MonoBehaviour
{
    public Character character_to_instantiate;
    public GameObject[] allCharacters;
    public CharactersManager charactersManager;

    void Start()
    {
        Events.OnGoalDone += OnSkip;
        Events.OnSkipOn(OnSkip);
        StartCoroutine(On());
    }
    void OnDestroy()
    {
        Events.OnGoalDone -= OnSkip;
    }
    IEnumerator On()
    {
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

        charactersManager.Init();

        for (int id = 0; id < totalCharacters; id++)
        {
            character = charactersManager.team1[id];
            character.gameObject.SetActive(true);

            character.transform.localScale = Vector3.one;
            character.actions.Goal();

            if(id == 0)
                SetCharacterOn(character);
        }

        SetGoalToCharacter();
        yield return new WaitForSeconds(7);
        Events.ChangeVolume("croud", 0.5f);
        OnSkip();
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
        Vector3 pos = characterToSwitch.transform.parent.transform.position;
        characterToSwitch.transform.parent.transform.position = character.transform.parent.transform.position;
        character.transform.parent.transform.position = pos;
        print(characterToSwitch.data.id + " __________________id: " + character.data.id);
    }
    void SetCharacterOn(Character character)
    {
        Events.SetDialogue(character, Data.Instance.textsData.GetRandomDialogue("goal", character.data.id, character.type == Character.types.GOALKEEPER));
    }
    void OnSkip()
    {
        StopAllCoroutines();
        Data.Instance.LoadLevel("Game");
        Events.OnSkipOff();
    }
}
