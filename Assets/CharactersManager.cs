using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
    public bool player1;
    public bool player2;
    public bool player3;
    public bool player4;

    public int totalPlayers = 0;
    int totalCharatersInTeam = 5;
    public List<Character> team1;
    public List<Character> team2;
    public List<Character> playingCharacters;
    int team_1_id = 0;
    int team_2_id = 0;

    public void AddCharacter(int characterID)
    {
        int teamID = 1;
        if (characterID  == 2 || characterID == 4) teamID = 2;

        if (characterID == 1) player1 = true;
        else if(characterID == 2) player2 = true;
        else if(characterID == 3) player3 = true;
        else if(characterID == 4) player4 = true;

        Character character = GetNextCharacterByTeam(teamID);
        character.id = characterID;
        playingCharacters.Add(character);
        totalPlayers++;
    }
    Character GetNextCharacterByTeam(int teamID)
    {
        switch(teamID)
        {
            case 1:
                team_1_id++;
                if (team_1_id > totalCharatersInTeam)   team_1_id = 1;
                return team1[team_1_id-1];
            case 2:
                team_2_id++;
                if (team_2_id > totalCharatersInTeam) team_2_id = 1;
                return team2[team_2_id-1];
        }
        return null;
    }
    public void SetPosition(int playerID, float _x, float _y)
    {
        Character character = GetPlayer(playerID);
        character.SetPosition((int)_x, (int)_y);
    }
    public void Kick(int playerID)
    {
        Character character = GetPlayer(playerID);
        character.Kick();
    }
    public void KickAllTheOthers(int characterID)
    {
        List<Character> team = team1;
        if (characterID == 2 || characterID == 4) team = team2;

        foreach (Character character in team)
            if (character.id == 0)
                character.Kick();
    }
    Character GetPlayer(int id)
    {
        foreach (Character character in playingCharacters)
            if (character.id == id)
                return character;
        return null;
    }
    
}
