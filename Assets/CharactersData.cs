using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersData : MonoBehaviour
{
    public List<GameObject> all;
    List<Sprite> thumbs;
    public List<int> team1;
    public List<int> team2;

    [HideInInspector] public List<int> availablesTeam1;
    [HideInInspector] public List<int> availablesTeam2;
    int totalCharacters;

    public void Init()
    {
        totalCharacters = 0;
        all.Clear();
        availablesTeam1.Clear();
        availablesTeam2.Clear();
        thumbs = new List<Sprite>();
        team1.Clear();
        team2.Clear();
        for (int a= 1; a<500; a++)
        {
            GameObject go = Resources.Load<GameObject>("players/" + a);
            if (go == null)
            {
                InitAll();
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
    private void InitAll()
    {
        
        for (int a = 1; a < totalCharacters+1; a++)
        {
            if (a <= (totalCharacters+1) / 2)
                availablesTeam1.Add(a);
            else
                availablesTeam2.Add(a);
        }
        Utils.Shuffle(availablesTeam1);
        Utils.Shuffle(availablesTeam2);
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
        print("team: " + teamID + " id: " + id);
        if(teamID == 1)
            return all[team1[id]-1];
        else
            return all[team2[id]-1];
    }
    public List<Sprite> GetAvailablePlayers(int teamID)
    {
        List<Sprite> allavailable = new List<Sprite>();
        if(teamID == 1)
        {
            foreach (int a in availablesTeam1)
                allavailable.Add(thumbs[a-1]);
        }
        else
        {
            foreach (int a in availablesTeam2)
                allavailable.Add(thumbs[a-1]);
        }        
        return allavailable;
    }
}
