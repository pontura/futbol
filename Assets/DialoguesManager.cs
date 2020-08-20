using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguesManager : MonoBehaviour
{
    public CharacterDialogueUI characterDialogueUI_to_instantiate;
    public List<CharacterDialogueUI> all;
    public Transform container;

    void Start()
    {
        Events.SetDialogue += SetDialogue;
        Invoke("LoopRandomDialogues", 5);
    }
    void OnDestroy()
    {
        Events.SetDialogue -= SetDialogue;
    }
    void LoopRandomDialogues()
    {
        Invoke("LoopRandomDialogues", 5);
        Character character;
        if(Random.Range(0,10)<5)
            character = Game.Instance.charactersManager.team1[Random.Range(0, Game.Instance.charactersManager.team1.Count)];
        else
            character = Game.Instance.charactersManager.team2[Random.Range(0, Game.Instance.charactersManager.team2.Count)];

        string text = Data.Instance.textsData.GetRandomDialogue("random", character.characterID);
        Events.SetDialogue(character, text);
    }
    void SetDialogue(Character character, string text)
    {
        CharacterDialogueUI c = GetDialogue(character);
        if(c == null)
        {
            c = Instantiate(characterDialogueUI_to_instantiate);
            c.transform.SetParent(container);
            c.transform.localScale = Vector3.one;
            all.Add(c);
        }
        c.Init(character, text);
        
    }
    CharacterDialogueUI GetDialogue(Character character)
    {
        foreach(CharacterDialogueUI c in all)
        {
            if (!c.enabled || c.character == character)
                return c;
        }
        return null;
    }
}
