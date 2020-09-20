using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersData : MonoBehaviour
{
    public List<GameObject> all;
    public List<GameObject> all_goalkeepers;
    public List<Sprite> thumbs;
    public List<Sprite> thumbs_goalkeepers;
    public List<int> team1;
    public List<int> team2;

    [HideInInspector] public List<int> availablesTeam1;
    [HideInInspector] public List<int> availablesTeam2;
    [HideInInspector] public List<int> availablesTeam1_goalkeepers;
    [HideInInspector] public List<int> availablesTeam2_goalkeepers;
    int totalCharacters;
    int totalGoalKeepers;

    public void Init()
    {
        totalCharacters = 0;
        all.Clear();
        availablesTeam1.Clear();
        availablesTeam2.Clear();

        availablesTeam1_goalkeepers.Clear();
        availablesTeam2_goalkeepers.Clear();

        thumbs = new List<Sprite>();
        thumbs_goalkeepers = new List<Sprite>();
        team1.Clear();
        team2.Clear();
        LoadPlayers();
    }
    void LoadPlayers()
    {
        for (int a = 1; a < 500; a++)
        {
            GameObject go = Resources.Load<GameObject>("players/" + a);
            if (go == null)
            {
                LoadGoalKeepers();
                return;
            }
            else
            {
                all.Add(go);
                Sprite s = Resources.Load<Sprite>("players_thumbnails/thumb_" + a) as Sprite;
                thumbs.Add(s);
                totalCharacters++;
            }
        }
    }
    void LoadGoalKeepers()
    {
        for (int a = 1; a < 500; a++)
        {
            GameObject go = Resources.Load<GameObject>("goalKeepers/" + a);
            if (go == null)
            {
                InitAll();
                return;
            }
            else
            {
                all_goalkeepers.Add(go);
                Sprite s = Resources.Load<Sprite>("players_thumbnails/thumb_goalkeeper_" + a) as Sprite;
                thumbs_goalkeepers.Add(s);
                totalGoalKeepers++;
            }
        }
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
        Utils.Shuffle(availablesTeam1);
        Utils.Shuffle(availablesTeam2);

        Utils.Shuffle(availablesTeam1_goalkeepers);
        Utils.Shuffle(availablesTeam2_goalkeepers);
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
                return all[team1[id] - 1];
            }
            else
            {
                return all_goalkeepers[team1[id] - 1];
            }
        else
             if (id < 4)
            {
                return all[team2[id] - 1];
            }
            else
            {
                return all_goalkeepers[team2[id] - 1];
            }
    }
    public List<Sprite> GetAvailablePlayers(int teamID, bool isGoalKeeper)
    {
        List<Sprite> allavailable = new List<Sprite>();
        if(teamID == 1)
        {
            if(isGoalKeeper)
                foreach (int a in availablesTeam1_goalkeepers)
                    allavailable.Add(thumbs_goalkeepers[a - 1]);
            else
                foreach (int a in availablesTeam1)
                    allavailable.Add(thumbs[a-1]);
        }
        else
        {
            if (isGoalKeeper)
                foreach (int a in availablesTeam2_goalkeepers)
                    allavailable.Add(thumbs_goalkeepers[a - 1]);
            else
                foreach (int a in availablesTeam2)
                    allavailable.Add(thumbs[a-1]);
        }        
        return allavailable;
    }
}
