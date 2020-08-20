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

    }
    void Start()
    {
        TextAsset targetFile = Resources.Load<TextAsset>("texts");
        data = JsonUtility.FromJson<TextData>(targetFile.text);
    }
    public string GetRandomDialogue(string dialogueType, int characterID)
    {
        CharactersData characterData = GetCharactersData(characterID);
        List<string> arr;
        switch (dialogueType)
        {
            case "random":
                if (UnityEngine.Random.Range(0, 10) < 4 && characterData.dialogs.random.Count > 0)
                    arr = characterData.dialogs.random;
                else
                    arr = data.dialogs.random;
                break;
            case "goal":
                if (UnityEngine.Random.Range(0, 10) < 4 && characterData.dialogs.goal.Count > 0)
                    arr = characterData.dialogs.goal;
                else
                    arr = data.dialogs.goal;
                break;
            default:
                if (UnityEngine.Random.Range(0, 10) < 4 && characterData.dialogs.full.Count > 0)
                    arr = characterData.dialogs.full;
                else
                    arr = data.dialogs.full;
                break;
        }
       
        return arr[UnityEngine.Random.Range(0, arr.Count)];
    }
    public CharactersData GetCharactersData(int characterID)
    {
        foreach (CharactersData data in data.characters)
        {
            if (data.id == characterID)
                return data;
        }
        return null;
    }
}
