using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharactersData : MonoBehaviour
{
    static CharactersData mInstance = null;
    public static CharactersData Instance
    {
        get {  return mInstance;   }
    }
    [Serializable]
    public class CharacterData
    {
        public GameObject asset;
        public Sprite thumb;
        public AudioClip[] audio_names;
        public AudioClip[] audio_goal;
    }
    public List<CharacterData> all;
    public List<CharacterData> all_goalkeepers;
    public List<CharacterData> all_referis;

    public List<int> team1;
    public List<int> team2;

    [HideInInspector] public List<int> availablesTeam1;
    [HideInInspector] public List<int> availablesTeam2;
    [HideInInspector] public List<int> availablesTeam1_goalkeepers;
    [HideInInspector] public List<int> availablesTeam2_goalkeepers;
    [HideInInspector] public List<int> availableReferis;

    int totalCharacters;
    int totalGoalKeepers;
    int totalReferis;
    public int referiId;

    void Awake()
    {
        if (!mInstance)
            mInstance = this;
        else
            Destroy(this.gameObject);
        DontDestroyOnLoad(this);
    }
    public void Init()
    {
        totalCharacters = all.Count;
        totalGoalKeepers = all_goalkeepers.Count;
        totalReferis = all_referis.Count;

        availablesTeam1.Clear();
        availablesTeam2.Clear();

        availablesTeam1_goalkeepers.Clear();
        availablesTeam2_goalkeepers.Clear();

        team1.Clear();
        team2.Clear();
        InitAll();
    }
    private void InitAll()
    {        
        for (int a = 1; a < totalCharacters+1; a++)
        {
            if (a <= (totalCharacters+1) / 2)
                availablesTeam1.Add(a);
            else
                availablesTeam2.Add(a);
        }
        for (int a = 1; a < totalGoalKeepers + 1; a++)
        {
            if (a <= (totalGoalKeepers + 1) / 2)
                availablesTeam1_goalkeepers.Add(a);
            else
                availablesTeam2_goalkeepers.Add(a);
        }
        for (int a = 1; a < totalReferis + 1; a++)
           availableReferis.Add(a);

        Utils.Shuffle(availablesTeam1);
        Utils.Shuffle(availablesTeam2);

        Utils.Shuffle(availablesTeam1_goalkeepers);
        Utils.Shuffle(availablesTeam2_goalkeepers);

        Utils.Shuffle(availableReferis);
    }
    public void AddCharacterToTeam(int teamID, int id)
    {
        if (teamID == 1)
        {
            team1.Add(id);
            availablesTeam1.Remove(id);
        }
        else
        {
            team2.Add(id);
            availablesTeam2.Remove(id);
        }
    }
    public GameObject GetCharacter(int teamID, int id)
    {
        if (teamID == 1)
            if (id < 4)
            {
                return all[team1[id] - 1].asset;
            }
            else
            {
                return all_goalkeepers[team1[id] - 1].asset;
            }
        else
             if (id < 4)
            {
                return all[team2[id] - 1].asset;
            }
            else
            {
                return all_goalkeepers[team2[id] - 1].asset;
            }
    }
    public List<Sprite> GetReferies()
    {
        List<Sprite> allavailable = new List<Sprite>();
        foreach (int a in availableReferis)
            allavailable.Add(all_referis[a - 1].thumb);
        return allavailable;
    }
    public List<Sprite> GetAvailablePlayers(int teamID, bool isGoalKeeper)
    {
        List<Sprite> allavailable = new List<Sprite>();
        if(teamID == 1)
        {
            if(isGoalKeeper)
                foreach (int a in availablesTeam1_goalkeepers)
                    allavailable.Add(all_goalkeepers[a - 1].thumb);
            else
                foreach (int a in availablesTeam1)
                    allavailable.Add(all[a-1].thumb);
        }
        else
        {
            if (isGoalKeeper)
                foreach (int a in availablesTeam2_goalkeepers)
                    allavailable.Add(all_goalkeepers[a - 1].thumb);
            else
                foreach (int a in availablesTeam2)
                    allavailable.Add(all[a-1].thumb);
        }

        return allavailable;
    }
}
