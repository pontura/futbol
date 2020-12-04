using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
    [HideInInspector] public int teamID_1;
    [HideInInspector] public int teamID_2;
    public float rotationOffset = 10;
    public Referi referi;
    public GameObject boardFloor;
    Vector2 limits;
    Ball ball;
    public bool player1;
    public bool player2;
    public bool player3;
    public bool player4;

    int totalPlayers = 0;
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
        Events.KickToGoal += KickToGoal;
    }
    private void OnDestroy()
    {
        Events.OnGoal -= OnGoal;
        Events.KickToGoal -= KickToGoal;
    }
    public void Init(int totalPlayersActive)
    {
        totalPlayers = CharactersData.Instance.team1.Count;
        teamID_1 = 1;
        teamID_2 = 2;
        ball = Game.Instance.ball;
        //AddCharacter(2);
        referi.InitReferi(this, CharactersData.Instance.all_referis[CharactersData.Instance.referiId-1].asset);
        limits = new Vector2(boardFloor.transform.localScale.x / 2, boardFloor.transform.localScale.z / 2);
        limits.x -= 1;
        int id = 0;
        foreach (Character character in containerTeam1.GetComponentsInChildren<Character>())
        {
            team1.Add(character);
            character.Init(teamID_1, this, CharactersData.Instance.GetCharacter(teamID_1, id, character.GetComponent<GoalKeeper>() != null));
            id++;
        }
        id = 0;
        foreach (Character character in containerTeam2.GetComponentsInChildren<Character>())
        {
            team2.Add(character);
            character.Init(teamID_2, this, CharactersData.Instance.GetCharacter(teamID_2, id, character.GetComponent<GoalKeeper>() != null));
            id++;
        }
        Loop();

        if (totalPlayersActive > 0)
        {
            AddCharacter(1);
            AddCharacter(2);
        }
        //AddCharacter(2);
    }
    public void InitPenalty(int totalPlayersActive)
    {
        teamID_1 = 1;
        teamID_2 = 2;
        if (Data.Instance.matchData.penaltyGoalKeeperTeamID == 1)
        {
            teamID_1 = 2;
            teamID_2 = 1;
        }
        
        ball = Game.Instance.ball;
        //AddCharacter(2);
        referi.InitReferi(this, CharactersData.Instance.all_referis[CharactersData.Instance.referiId - 1].asset);
        limits = new Vector2(boardFloor.transform.localScale.x / 2, boardFloor.transform.localScale.z / 2);
        limits.x -= 1;

        //patea:
        Character character = containerTeam1.GetComponentInChildren<Character>();
        team1.Add(character);
        character.Init(teamID_1, this, CharactersData.Instance.GetCharacter(teamID_1, Random.Range(0, CharactersData.Instance.team2.Count-2), false));
        character.actions.LookTo(-1); //mira a la izquierda siempre:
        Events.OnPenaltyWaitingToKick(character, GetComponent<Penalty>().PenaltyPita);

        //ataja:
        character = containerTeam2.GetComponentInChildren<Character>();
        team2.Add(character);
        character.Init(teamID_2, this, CharactersData.Instance.GetCharacter(teamID_2, CharactersData.Instance.team2.Count-1, true));

        if (totalPlayersActive > 0)
        {
            AddCharacter(1);
        }
        
    }
    public void ResetAll()
    {
        foreach (Character c in team1)
        {
            c.ai.ResetPosition();
            c.actions.LookAtBall();
            c.actions.Idle();
        }
        foreach (Character c in team2)
        {
            c.ai.ResetPosition();
            c.actions.LookAtBall();
            c.actions.Idle();
        }
    }
    void Loop()
    {
        CheckStateByTeam(team1[0]);
        CheckStateByTeam(team2[0]);
        Invoke("Loop", Data.Instance.settings.gameplay.timeToSwapCharactersAutomatically);
    }
    void CheckStateByTeam(Character character)
    {
        AI.states state = character.ai.state;
        if (state != AI.states.ATTACKING)
        {
            //SwapIfNeeded(character.teamID);
            CheckForNewDefender(character.teamID);
        }
    }
    void SwapIfNeeded(int teamID)
    {
        Character to = GetNearest(teamID, false, ball.transform.position);
        Character from = GetNearest(teamID, true, ball.transform.position);
        if (to == null || from == null)
            return;
        if (to != from && to.id != from.id)
            SwapTo(from, to);
    }
    void CheckForNewDefender(int teamID)
    {
        Character nearestToDefend = GetNearest(teamID, false, ball.transform.position);
        if (ball.character != null && ball.character.isGoalKeeper)
            return;
        if (nearestToDefend.isBeingControlled || nearestToDefend.ai.aiGotoBall.enabled)
            return;
        Events.SetCharacterNewDefender(nearestToDefend);
    }
    public void CharacterCatchBall(Character character)
    {
        if (character.isBeingControlled)
            return;
        Character characterNear = GetNearest(character.teamID, true, ball.transform.position);
        SwapTo(characterNear, character);
    }
    public void AddCharacter(int id)
    {
        int teamID = GetTeamByPlayer(id);

        if (id == 1) player1 = true;
        else if(id == 2) player2 = true;
        else if(id == 3) player3 = true;
        else if(id == 4) player4 = true;

        Character character = GetNextCharacterByTeam(teamID);
        character.id = id;
        playingCharacters.Add(character);
        signals.Add(character);
        character.SetControlled(true);
    }
    int GetTeamByPlayer(int characterID)
    {
        int teamID = 1;
        if (characterID == 2 || characterID == 4) teamID = 2;
        return teamID;
    }
    //hasControl = if true solo busca entre los activos.
    public Character GetNearestTo(Character myCharacter, int teamID)
    {
        List<Character> team;
        if (teamID == 1)
            team = team1;
        else team = team2;
        float distanceMin = 1000;
        Character character = null;
        foreach (Character c in team)
        {
            if (c.characterID != myCharacter.characterID)
            {
                float distance = Vector3.Distance(c.transform.position, myCharacter.transform.position);
                if (distanceMin > distance && !c.isGoalKeeper)
                {
                    character = c;
                    distanceMin = distance;
                }
            }
        }
        return character;
    }
    public Character GetNearest(int teamID, bool hasControl, Vector3 pos)
    {
        List<Character> team;
        if (teamID == 1)
            team = team1;
        else team = team2;
        float distanceMin = 1000;
        Character character = null;
        foreach (Character c in team)
        {
            float distance = Vector3.Distance(c.transform.position, pos);
            if (distanceMin > distance && !c.isGoalKeeper)
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
                if (team_1_id > totalPlayers) team_1_id = 1;
                return team1[team_1_id - 1];
            case 2:
                team_2_id++;
                if (team_2_id > totalPlayers) team_2_id = 1;
                return team2[team_2_id - 1];
        }
        return null;    
    }
    public void SetPosition(int playerID, float _x, float _y)
    {
        Character character = GetPlayer(playerID);
        if (character.transform.position.x >= limits.x && _x>0 || character.transform.position.x <= -limits.x && _x < 0) _x = 0;
        if (character.transform.position.z >= limits.y && _y > 0 || character.transform.position.z <= -limits.y && _y < 0) _y = 0;
        character.SetPosition(_x, _y);
        Vector3 rot = character.characterContainer.transform.localEulerAngles;
        rot.y += transform.position.x * rotationOffset;
        character.characterContainer.transform.localEulerAngles = rot;
    }

    public void ButtonPressed(int buttonID, int id)
    {
       // print("buttonID " + buttonID);
        Character character = GetPlayer(id);

        if (buttonID == 3)
        {
            character.SuperRun();
            return;
        }           

        if (ball.character == null || ball.character != character)
        {
            switch (buttonID)
            {
                case 1: character.Dash(); break;
                case 2: Swap(id); break;
                    // case 3: KickAllTheOthers(characterID); break;
            }
        }
        else if (ball.character == character)
        {
           Events.PlayerProgressBarSetState(true);
        }
    }   
    public void ButtonUp(int buttonID, int id)
    {
       
        Character character = GetPlayer(id);
        if (character.ai.state == AI.states.ATTACKING)
        {
            Events.PlayerProgressBarSetState(false);
            switch (buttonID)
            {
                case 1:
                    float timeCatched = ball.GetDurationOfBeingCatch();

                    if (timeCatched < 0.25f)
                        Events.KickToGoal();
                    else
                        character.Kick(CharacterActions.kickTypes.HARD);

                    break;
                case 2:
                    //  Character characterNear = GetNearest(character.teamID, false, ball.transform.position + ball.transform.forward * 4);
                    Character characterNear = GetNearestTo(character, character.teamID);

                    if (ball.character != characterNear)
                    {
                        print("a: " + characterNear.data.avatarName);
                        character.ballCatcher.LookAt(characterNear.transform.position);
                        SwapTo(character, characterNear);
                    }
                    CheckPase(character);
                    break;
                case 3: return;// character.Kick(CharacterActions.kickTypes.BALOON); break;
            }
        }
    }
    void CheckPase(Character character)
    {
        float uiForceValue = ball.uIForce.GetForce();
        if (uiForceValue > 0.5f)
            character.Kick(CharacterActions.kickTypes.BALOON);
        else
            character.Kick(CharacterActions.kickTypes.SOFT);
    }
    public void Swap(int characterID)
    {
        int teamID = GetTeamByPlayer(characterID);
        Character character = GetPlayer(characterID);       
        Character newCharacter = GetNearest(teamID, false, ball.transform.position);
        if (newCharacter != character)
            SwapTo(character, newCharacter);
    }
    public void SwapTo(Character from, Character to)
    {
        if (to == null) return;
        if (from == null) return;

        Events.PlaySound("common", "swapPlayer", false);

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
        Events.SwapCharacter(to);
    }
    public void OnGoal(int teamID, Character c)
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
    void KickToGoal()
    {
        if(ball.character!= null)
            ball.character.Kick(CharacterActions.kickTypes.KICK_TO_GOAL);
    }


}
