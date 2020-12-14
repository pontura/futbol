using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextsData : MonoBehaviour
{
    public TextData data;

    [Serializable]
    public class TextData
    {
        public DialoguesData dialogs;
        public CharacterData[] characters;
        public CharacterData[] goalkeepers;
        public CharacterData[] referis;
    }
    [Serializable]
    public class CharacterData
    {
        public int id;
        public string avatarName;
        public DialoguesData dialogs;
    }

    [Serializable]
    public class DialoguesData
    {
        public List<string> random;
        public List<string> goal;
        public List<string> full;
        public List<string> init;
    }
    void Awake()
    {
        TextAsset targetFile = Resources.Load<TextAsset>("texts");
        data = JsonUtility.FromJson<TextData>(targetFile.text);        
    }
    public string GetRandomReferiDialogue(string dialogueType)
    {
        CharacterData characterData = data.referis[CharactersData.Instance.referiId - 1];
        foreach (CharacterData d in data.referis)
        {
            if (d.id == CharactersData.Instance.referiId)
                characterData = d;
        }
        return GetText(dialogueType, characterData, false);
    }
    public string GetRandomDialogue(string dialogueType, int characterID, bool isGoalKeeper = false)
    {
        CharacterData characterData = GetCharactersData(characterID, isGoalKeeper);
        return GetText(dialogueType, characterData, true);
    }
    string GetText(string dialogueType, CharacterData characterData, bool isPlayer)
    {
        List<string> arr = new List<string>();
        switch (dialogueType)
        {
            case "random":
                if (characterData.dialogs.random.Count > 0 || !isPlayer)
                    arr = characterData.dialogs.random;
                break;
            case "goal":
                if (UnityEngine.Random.Range(0, 10) < 4 && characterData.dialogs.goal.Count > 0 || !isPlayer)
                    arr = characterData.dialogs.goal;
                else
                    arr = data.dialogs.goal;
                break;
            case "init":
                if (UnityEngine.Random.Range(0, 10) < 4 && characterData.dialogs.init.Count > 0 || !isPlayer)
                    arr = characterData.dialogs.init;
                else
                    arr = data.dialogs.init;
                break;
            default:
                if (UnityEngine.Random.Range(0, 10) < 4 && characterData.dialogs.full.Count > 0 || !isPlayer)
                    arr = characterData.dialogs.full;
                else
                    arr = data.dialogs.full;
                break;
        }
        if (arr == null || arr.Count == 0)
            return null;
        return arr[UnityEngine.Random.Range(0, arr.Count)];
    }
    public CharacterData GetCharactersData(int characterID, bool isGoalKeeper = false)
    {
        if (isGoalKeeper)
        {
            foreach (CharacterData data in data.goalkeepers)
            {
                if (data.id == characterID)
                    return data;
            }
        }
        foreach (CharacterData data in data.characters)
        {
            if (data.id == characterID)
                return data;
        }
        return null;
    }
    public CharacterData GetReferisData(int characterID)
    {
        foreach (CharacterData data in data.referis)
        {
            if (data.id == characterID)
                return data;
        }
        return null;
    }
}
