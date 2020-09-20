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
        public CharactersData[] characters;
        public CharactersData[] goalkeepers;
        public CharactersData[] referis;
    }
    [Serializable]
    public class CharactersData
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
        int referiID = 1;
        CharactersData characterData = data.referis[0];
        foreach (CharactersData d in data.referis)
        {
            if (d.id == referiID)
                characterData = d;
        }
        return GetText(dialogueType, characterData, false);        
    }   
    public string GetRandomDialogue(string dialogueType, int characterID, bool isGoalKeeper = false)
    {
        CharactersData characterData = GetCharactersData(characterID, isGoalKeeper);
        return GetText(dialogueType, characterData, true);
    }
    string GetText(string dialogueType, CharactersData characterData, bool isPlayer)
    {
        List<string> arr;
        switch (dialogueType)
        {
            case "random":
                if (UnityEngine.Random.Range(0, 10) < 4 && characterData.dialogs.random.Count > 0 || !isPlayer)
                    arr = characterData.dialogs.random;
                else
                    arr = data.dialogs.random;
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
            return "AMOR";
        return arr[UnityEngine.Random.Range(0, arr.Count)];
    }
    public CharactersData GetCharactersData(int characterID, bool isGoalKeeper = false)
    {
        if(isGoalKeeper)
        {
            foreach (CharactersData data in data.goalkeepers)
            {
                if (data.id == characterID)
                    return data;
            }
        }
        foreach (CharactersData data in data.characters)
        {
            if (data.id == characterID)
                return data;
        }
        return null;
    }
}
