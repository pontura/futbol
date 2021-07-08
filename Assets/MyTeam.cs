using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTeam : MonoBehaviour
{
    public List<int> characters;
    public List<int> goalkeepers;

    public string saved_characters;
    public string saved_goalkeepers;

    private void Start()
    {
        LoadSavedData();
    }
    void LoadSavedData()
    {
        saved_characters = PlayerPrefs.GetString("saved_characters");
        saved_goalkeepers = PlayerPrefs.GetString("saved_goalkeepers");

        string[] all = saved_characters.Split(","[0]);
        if(all.Length>0)  foreach (string s in all)  characters.Add(int.Parse(s));

        all = saved_goalkeepers.Split(","[0]);
        if (all.Length > 0) foreach (string s in all) goalkeepers.Add(int.Parse(s));
    }
    public void SetCharacter(int id, bool add)
    {
        if (add)
            characters.Add(id);
        else
            characters.Remove(id);
    }
    public void SetCGeoalkeeper(int id, bool add)
    {
        if (add)
            goalkeepers.Add(id);
        else
            goalkeepers.Remove(id);
    }
    public void Save()
    {
        saved_characters = "";
        int a = 0;
        foreach (int id in characters)
        {
            a++;
            saved_characters += id.ToString();
            if(a<characters.Count)
                saved_characters += ",";
        }
        a = 0;
        foreach (int id in goalkeepers)
        {
            a++;
            saved_goalkeepers += id.ToString();
            if (a < goalkeepers.Count)
                saved_goalkeepers += ",";
        }

        PlayerPrefs.SetString("saved_characters", saved_characters);
        PlayerPrefs.SetString("saved_goalkeepers", saved_goalkeepers);
    }
}
