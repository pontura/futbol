using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
    public List<GameObject> all;

    public GameObject boardFloor;
    Vector2 limits;
    public Ball ball;
    public bool player1;
    public bool player2;
    public bool player3;
    public bool player4;

    public int totalPlayers = 0;
    int totalCharatersInTeam = 5;
    public GameObject containerTeam1, containerTeam2;
    public List<Character> team1;
    public List<Character> team2;
    public List<Character> playingCharacters;
    int team_1_id = 0;
    int team_2_id = 0;
    public CharactersSignals signals;

    private void Awake()
    {
        Events.OnGoal += OnGoal;
    }
    private void OnDestroy()
    {
        Events.OnGoal -= OnGoal;
    }
    private void Start()
    {
        limits = new Vector2(boardFloor.transform.localScale.x / 2, boardFloor.transform.localScale.z / 2);
        limits.x -= 1;
        foreach (Character character in containerTeam1.GetComponentsInChildren<Character>())
        {
            team1.Add(character);
            character.Init(1, this, all[Random.Range(0, all.Count)] );
        }
        foreach (Character character in containerTeam2.GetComponentsInChildren<Character>())
        {
            team2.Add(character);
            character.Init(2, this, all[Random.Range(0, all.Count)]);
        }
        Loop();
    }
    public void ResetAll()
    {
        foreach (Character c in team1)
        {
            c.ai.ResetPosition();
            c.actions.LookAtBall();
           
        }
        foreach (Character c in team2)
        {
            c.ai.ResetPosition();
            c.actions.LookAtBall();
        }
    }
    void Loop()
    {
        CheckStateByTeam(team1[0]);
        CheckStateByTeam(team2[0]);
        Invoke("Loop", 0.25f);
    }
    void CheckStateByTeam(Character character)
    {
        AI.states state = character.ai.state;
        if (state != AI.states.ATTACKING)
        {
            SwapIfNeeded(character.teamID);
        }
    }
    void SwapIfNeeded(int teamID)
    {
        Character to = GetNearest(teamID);
        Character from = GetNearest(teamID, true);
        if (to != from)
            SwapTo(from, to);
    }
    public void CharacterCatchBall(Character character)
    {
        if (character.isBeingControlled)
            return;
        Character characterNear = GetNearest(character.teamID, true);
        SwapTo(characterNear, character);
    }
    public void AddCharacter(int characterID)
    {
        int teamID = GetTeamByPlayer(characterID);

        if (characterID == 1) player1 = true;
        else if(characterID == 2) player2 = true;
        else if(characterID == 3) player3 = true;
        else if(characterID == 4) player4 = true;

        Character character = GetNextCharacterByTeam(teamID);
        character.id = characterID;
        playingCharacters.Add(character);
        totalPlayers++;
        signals.Add(character);
        character.SetControlled(true);
    }
    int GetTeamByPlayer(int characterID)
    {
        int teamID = 1;
        if (characterID == 2 || characterID == 4) teamID = 2;
        return teamID;
    }
    Character GetNearest(int teamID, bool hasControl = false)
    {
        List<Character> team;
        if (teamID == 1)
            team = team1;
        else team = team2;
        float distanceMin = 1000;
        Character character = null;
        foreach (Character c in team)
        {
            float distance = Vector3.Distance(c.transform.position, ball.transform.position);
            if (distanceMin > distance && !c.isGoldKeeper)
            {
                if (hasControl)
                {
                    if (c.isBeingControlled)
                    {
                        character = c;
                        distanceMin = distance;
                    }
                }
                else
                {
                    character = c;
                    distanceMin = distance;
                }
            }
        }
        return character;
    }
    Character GetNextCharacterByTeam(int teamID)
    {
        switch (teamID)
        {
            case 1:
                team_1_id++;
                if (team_1_id > totalCharatersInTeam) team_1_id = 1;
                return team1[team_1_id - 1];
            case 2:
                team_2_id++;
                if (team_2_id > totalCharatersInTeam) team_2_id = 1;
                return team2[team_2_id - 1];
        }
        return null;    
    }
    public void SetPosition(int playerID, float _x, float _y)
    {
        Character character = GetPlayer(playerID);
        if (character.transform.position.x >= limits.x && _x>0 || character.transform.position.x <= -limits.x && _x < 0) _x = 0;
        if (character.transform.position.z >= limits.y && _y > 0 || character.transform.position.z <= -limits.y && _y < 0) _y = 0;
        character.SetPosition((int)_x, (int)_y);
    }
    public void GoalKeeperLoseBall(int characterID)
    {
        Swap(characterID);
    }
    public void ButtonPressed(int buttonID, int characterID)
    {
        Character character = GetPlayer(characterID);
        if (character.ai.state == AI.states.ATTACKING)
        {
            switch (buttonID)
            {
                case 1: character.Kick(CharacterActions.kickTypes.HARD); break;
                case 2: character.Kick(CharacterActions.kickTypes.SOFT); break;
                case 3: character.Kick(CharacterActions.kickTypes.BALOON); break;
            }
            if(character.isGoldKeeper)
                GoalKeeperLoseBall(characterID);
        }
        else if (character.ai.state == AI.states.DEFENDING || character.ai.state == AI.states.NONE)
        {
            switch (buttonID)
            {
                case 1: character.Dash(); break;
                case 2: Swap(characterID); break;
               // case 3: KickAllTheOthers(characterID); break;
            }
        }
    }   
    public void Swap(int characterID)
    {
        int teamID = GetTeamByPlayer(characterID);
        Character character = GetPlayer(characterID);       
        Character newCharacter = GetNearest(teamID, false);
        if (newCharacter != character)
            SwapTo(character, newCharacter);
    }
    public void SwapTo(Character from, Character to)
    {
        if (to == null) return;
        if (from == null) return;
        int teamID = from.teamID;
        to.id = from.id;        
        signals.ChangeSignal(from, to);

        from.id = 0;
        playingCharacters.Remove(from);
        playingCharacters.Add(to);
        from.SetControlled(false);
        to.SetControlled(true);
        to.actions.Reset();
        from.actions.Reset();
    }
    public void OnGoal(int teamID)
    {
        List<Character> team_win;
        List<Character> team_lose;
        if (teamID == 1)
        {
            team_win = team1;
            team_lose = team2;
        }
        else
        {
            team_win = team2;
            team_lose = team1;
        }

        foreach (Character character in team_win)
            character.OnGoal(true);
        foreach (Character character in team_lose)
            character.OnGoal(false);
    }
    Character GetPlayer(int id)
    {
        foreach (Character character in playingCharacters)
            if (character.id == id)
                return character;
        return null;
    }
    
}
