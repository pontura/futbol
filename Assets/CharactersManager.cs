﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : MonoBehaviour
{
    [HideInInspector] public int teamID_1;
    [HideInInspector] public int teamID_2;
    public float rotationOffset = 10;
    public Referi referi;
    Vector2 limits;
    Ball ball;

    int totalPlayers = 0;
    public GameObject containerTeam1, containerTeam2;
    public List<Character> team1;
    public List<Character> team2;
    public List<Character> playingCharacters;
    int team_1_id = 0;
    int team_2_id = 0;
    public CharactersSignals signals;
    Settings.GamePlay gameplaySettings;

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
    public void Init(bool reverseTeams = false)
    {
        CharactersConstructor cc = GetComponent<CharactersConstructor>();
        if(cc != null)  cc.AddCharacters();

        gameplaySettings = Data.Instance.settings.gameplay;
        totalPlayers = CharactersData.Instance.team1.Count;
        if (reverseTeams)
        {
            teamID_1 = 2;
            teamID_2 = 1;
        } else
        {
            teamID_1 = 1;
            teamID_2 = 2;
        }
        ball = Game.Instance.ball;
        //AddCharacter(2);
        referi.InitReferi(this, CharactersData.Instance.all_referis[CharactersData.Instance.referiId-1].asset);
        limits = new Vector2(Data.Instance.stadiumData.active.size_x / 2, Data.Instance.stadiumData.active.size_y / 2);
        limits.x -= 1;
        int id = 0;
        foreach (Character character in containerTeam1.GetComponentsInChildren<Character>())
        {
            team1.Add(character);
            character.Init(teamID_1, this, CharactersData.Instance.GetCharacter(teamID_1, id, character.type == Character.types.GOALKEEPER));
            id++;
        }
        id = 0;
        foreach (Character character in containerTeam2.GetComponentsInChildren<Character>())
        {
            team2.Add(character);
            character.Init(teamID_2, this, CharactersData.Instance.GetCharacter(teamID_2, id, character.type == Character.types.GOALKEEPER));
            id++;
        }
        
        print("Data.Instance.newScene: " + Data.Instance.newScene);
        if (Data.Instance.newScene == "Game")
        {
            Loop();
            for (int a = 0; a < Data.Instance.matchData.players.Length; a++)
            {
                int teamID = Data.Instance.matchData.players[a];
                if (teamID != 0) AddCharacter(a + 1, teamID);
            }
        }
            

        ResetAll();
        if (Data.Instance.settings.mainSettings.turn_on_only_other_goalkeeper)
        {
            foreach (Character ch in team2)
                if(ch.type != Character.types.GOALKEEPER)
                    ch.gameObject.SetActive(false);
        }
        else if (Data.Instance.settings.mainSettings.turn_off_team2)
            containerTeam2.SetActive(false);

        if (Data.Instance.newScene == "Game")
        {
            foreach (Character ch in team1)
                ch.SetOponent();
            foreach (Character ch in team2)
                ch.SetOponent();
        }
    }
    public void InitPenalty()
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
        limits = new Vector2(Data.Instance.stadiumData.active.size_x / 2, Data.Instance.stadiumData.active.size_y / 2);
        limits.x -= 1;

        //patea:
        Character character = containerTeam1.GetComponentInChildren<Character>();
        team1.Add(character);
        character.Init(teamID_1, this, CharactersData.Instance.GetCharacter(teamID_1, Random.Range(1, CharactersData.Instance.team2.Count-1), false));
        character.actions.LookTo(-1); //mira a la izquierda siempre:
        Events.OnPenaltyWaitingToKick(character, GetComponent<Penalty>().PenaltyPita);

        //ataja:
        character = containerTeam2.GetComponentInChildren<Character>();
        team2.Add(character);
        character.Init(teamID_2, this, CharactersData.Instance.GetCharacter(teamID_2, 0, true));

        AddCharacter(1, 1);
    }
    Character GetSacaCharacter(List<Character> team)
    {
        foreach (Character ch in team)
            if (ch.type == Character.types.DELANTERO)
                return ch;
        return null;
    }
    public void ResetAll()
    {
        List<Character> team;
        if (Data.Instance.matchData.lastGoalBy == 1)
            team = team2;
        else
            team = team1;        

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
        Character saca = GetSacaCharacter(team);
        if (saca != null)
        {
            AI ai = GetSacaCharacter(team).ai;
            if (ai != null)
                ai.SetInitialCharacterPosition();
        }
    }
    void Loop()
    {     
        Invoke("Loop", Data.Instance.settings.timeToSwapCharactersAutomatically);
        if (Game.Instance.state == Game.states.PLAYING)
        {
            CheckStateByTeam(team1[0]);
            CheckStateByTeam(team2[0]);
            BallInsideChecker();
           // CheckPosicionAdelantadas();
        }            
    }
    void  BallInsideChecker()
    {
        Vector3 ballPos = ball.transform.position;
        if (ball.transform.position.x > limits.x)
            ballPos.x = limits.x;
        else if (ball.transform.position.x < -limits.x)
            ballPos.x = -limits.x;
        ball.transform.position = ballPos;
    }
    void CheckStateByTeam(Character character)
    {
        if (Game.Instance.state != Game.states.PLAYING) return;
        if (ball.character == null)
        {
            if (character.teamID == 1 && Data.Instance.matchData.team1Controlled) SwapIfNeeded(1);
            else if (character.teamID == 2 && Data.Instance.matchData.team2Controlled) SwapIfNeeded(2);
        }
        else if (ball.character.teamID != character.teamID)
        {
            if (character.teamID == 1 && Data.Instance.matchData.team1Controlled) SwapIfNeeded(1);
            else if (character.teamID == 2 && Data.Instance.matchData.team2Controlled) SwapIfNeeded(2);
            else CheckForNewDefender(character.teamID);
        }
    }
    public void SwapIfNeeded(int teamID) // busca si hay otro cerca de la pelota y le da control
    {
        Character character = GetCharacterControlledFarest(teamID, ball.transform.position);
        Character newCharacter = GetNearest(teamID, false, ball.transform.position, false, false, true);
        //print("from: " + character.data.avatarName + "   to: " + newCharacter.data.avatarName);
        if (newCharacter.data.id != character.data.id)
            SwapTo(character, newCharacter);
    }
    void CheckForNewDefender(int teamID)
    {
        float offset = 2;
        Vector3 ballPos = ball.transform.position;
        if (teamID == 1)  ballPos.x += offset;   else  ballPos.x -= offset;

        Character nearestToDefend = GetNearest(teamID, false, ballPos, false, true);
        //if (nearestToDefend.ai.aiStateName == "AiGotoBall")
        //    return;
        if (ball.character != null && ball.character.type == Character.types.GOALKEEPER)
            return;
        if (nearestToDefend.isBeingControlled)
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
    public void AddCharacter(int id, int teamID)
    {
        Character character = GetNextCharacterByTeam(teamID);
        character.control_id = id;
        playingCharacters.Add(character);
        signals.Add(character);
        character.SetControlled(true);
    }
    int GetTeamByPlayer(int id)
    {
        return Data.Instance.matchData.players[id - 1];
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
            if (c.data.id != myCharacter.data.id)
            {
                float distance = Vector3.Distance(c.transform.position, myCharacter.transform.position);
                if (distanceMin > distance && c.type != Character.types.GOALKEEPER)
                {
                    character = c;
                    distanceMin = distance;
                }
            }
        }
        return character;
    }
    public Character GetCharacterControlledFarest(int teamID, Vector3 pos)
    {
        List<Character> team;
        if (teamID == 1)
            team = team1;
        else team = team2;
        float distanceNearest = 0;
        Character character = null;
        foreach (Character c in team)
        {
            float dist = Vector3.Distance(c.transform.position, pos);
            if (c.isBeingControlled && dist > distanceNearest)
            {
                character = c;
                distanceNearest = dist;
            }
        }
        return character;
    }
    public Character GetOtherCharacterNear(int teamID, Vector3 pos, float MaxDistance)
    {
        Character ch = GetNearest(teamID, false, pos);
        float distance = Vector3.Distance(pos, ch.transform.position);
        if (distance < MaxDistance)
            return ch;
        return null;
    }
    public Character GetNearest(int teamID, bool hasControl, Vector3 pos, bool ifHasControlGetSecond = false, bool DontGetGoalKeeper = false, bool ignoreControlls = false)
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
            if( DontGetGoalKeeper && c.type == Character.types.GOALKEEPER)
            {

            } else
            if (distanceMin > distance  )
            {
                if(ifHasControlGetSecond)
                {
                    if (!c.isBeingControlled)
                    {
                        character = c;
                        distanceMin = distance;
                    }
                } else if(ignoreControlls)
                {
                    character = c;
                    distanceMin = distance;
                }
                if (hasControl && c.isBeingControlled)
                {
                    character = c;
                    distanceMin = distance;
                }
                else if (!hasControl && !c.isBeingControlled)
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
    bool CantControl(int teamID)
    {
        if (Game.Instance.ball != null
            && Game.Instance.ball.character != null
            && Game.Instance.ball.character.type == Character.types.GOALKEEPER
            && Game.Instance.ball.character.teamID != teamID)
            return true;
        return false;
    }
    public void SetPosition(int playerID, float _x, float _y)
    {

        Character character = GetPlayer(playerID);
        if (character == null)
            return;

        if ((_x != 0 || _y != 0) && CantControl(character.teamID)) return;// no te deja controlar al avatar si la tiene el arquero contrario:

        if (character.transform.position.x >= limits.x && _x>0 || character.transform.position.x <= -limits.x && _x < 0) _x = 0;
        if (character.transform.position.z >= limits.y && _y > 0 || character.transform.position.z <= -limits.y && _y < 0) _y = 0;
        character.SetPosition(_x, _y);
        Vector3 rot = character.characterContainer.transform.localEulerAngles;
        rot.y += transform.position.x * rotationOffset;
        character.characterContainer.transform.localEulerAngles = rot;
    }
    public void Palancazo(int control_id)
    {
        Character character = GetPlayer(control_id);
        if (character == null) return;
        if (ball.character == character)
            character.Dash_with_ball();
    }
    public void ButtonPressed(int buttonID, int control_id)
    {
        Character character = GetPlayer(control_id);
        if (character == null) return;
        if (buttonID == 3)
        {
            if (character.actions.state == CharacterActions.states.IDLE)
            {
                if (ball.character == character)
                    character.Jueguito();
                else
                    character.Jump();
            }
            else
                character.SuperRun();
            return;
        }           

        if (ball.character == null || ball.character != character)
        {
            switch (buttonID)
            {
                case 1: character.Dash(); break;
                //case 2: Swap(control_id); break;
                case 2: character.Hit(); break;
                    // case 3: KickAllTheOthers(characterID); break;
            }
        }
        else if (ball.character == character)
        {
           Events.PlayerProgressBarSetState(true);
            character.StartKicking(buttonID);
        }
    }   
    public void ButtonUp(int buttonID, int id)
    {       
        Character character = GetPlayer(id);
        if(character == null || character.actions.state == CharacterActions.states.DASH_WITH_BALL)
        {
            Debug.Log("no hay usuario id: " + id);
            return;
        }
        if(ball.character != null && ball.character.teamID == character.teamID)
     //   if (character.ai.state == AI.states.ATTACKING)
        {
            Events.PlayerProgressBarSetState(false);
            switch (buttonID)
            {
                //arco
                case 1:
                    if (character.actions.state == CharacterActions.states.DASH) return;
                    float timeCatched = ball.GetDurationOfBeingCatch();

                    if (timeCatched < 0.25f)
                        Events.KickToGoal();
                    else
                        character.Kick(CharacterActions.kickTypes.HARD);

                    break;
                //pasa:
                case 2:
                    if (character.GetPosition() == Character.PositionsInGame.CENTRO)
                    {                      
                        Vector3 centroPos = character.transform.position;
                        centroPos.x *= 1.2f;
                        centroPos.z *= -0.85f;
                        character.ballCatcher.LookAt(centroPos);
                        character.Kick(CharacterActions.kickTypes.CENTRO);
                        return;
                    }
                    Pasar(character);
                    break;
                case 3: return;// character.Kick(CharacterActions.kickTypes.BALOON); break;
            }
        }
    }
    public Character GetCharacterEnPase(Character character)
    {
        return GetOtherCharacterNear(character.teamID, ball.GetForwardPosition(5), 7);
    }
    public void Pasar(Character character)
    {
        Character characterNear = GetCharacterEnPase(character);
        if (characterNear != null)
        {
            character.ballCatcher.LookAt(characterNear.transform.position);
            SwapTo(character, characterNear);
            float dist = Vector3.Distance(character.transform.position, characterNear.transform.position);
            character.Kick(CharacterActions.kickTypes.SOFT, 1 + (dist / 10));
            ball.PaseTo(characterNear);
        }
        else
        {
            float uiForceValue = character.ballCatcher.GetForce();
            if (uiForceValue > 0.6f)
                character.Kick(CharacterActions.kickTypes.BALOON);
            else
                character.Kick(CharacterActions.kickTypes.SOFT);
        }
    }
    public void Swap(int control_id)
    {
        int teamID = GetTeamByPlayer(control_id);
        Character character = GetPlayer(control_id);       
        Character newCharacter = GetNearest(teamID, false, ball.transform.position, true);
        if (newCharacter != character)
            SwapTo(character, newCharacter);
    }
    
    public void SwapTo(Character from, Character to)
    {
        if (to == null || to.isBeingControlled) return;
        if (from == null) return;
        if (from.teamID != to.teamID) return;
        if (from.control_id == to.control_id) return;

        Events.PlaySound("common", "swapPlayer", false);

        int teamID = from.teamID;
        to.control_id = from.control_id;        
        signals.ChangeSignal(from, to);

        from.control_id = 0;
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
    Character GetPlayer(int control_id)
    {
        foreach (Character character in playingCharacters)
            if (character.control_id == control_id)
                return character;
        return null;
    }
    void KickToGoal()
    {
        if(ball.character!= null)
            ball.character.Kick(CharacterActions.kickTypes.KICK_TO_GOAL);
    }
    public List<Character> GetCharactersByTeam(int teamID)
    {
        if (teamID == 1) return team1; else return team2;
    }
    public Character GetCharacterByType(Character.types type, int teamID)
    {
        List<Character> team;
        if (teamID == 1) team = team1; else team = team2;
        foreach(Character character in team)
        {
            if (character.type == type)
                return character;
        }
        return null;
    }

    float posicionAdelantadaTeam1 = 0;
    float posicionAdelantadaTeam2 = 0;
    public float GetPosicionAdelantada(int teamID)
    {
        if (teamID == 1) return posicionAdelantadaTeam1;
        else return posicionAdelantadaTeam2;
    }
    void CheckPosicionAdelantadas()
    {
        posicionAdelantadaTeam1 = GetPosicionAdelantadaByTeam(1);
        posicionAdelantadaTeam2 = GetPosicionAdelantadaByTeam(2);
    }
    float GetPosicionAdelantadaByTeam(int teamID)
    {
        float ball_x = ball.transform.position.x;
        float pos = ball_x;

        if (teamID == 1) { //se fija a la izquierda
            foreach (Character c in team2)
            {
                if (c.type != Character.types.GOALKEEPER)
                {
                    float character_x = c.transform.position.x;
                    if (character_x < pos)
                        pos = character_x;
                }
            }
        } else  {      //se fija a la derecha
            foreach (Character c in team1)
            {
                if (c.type != Character.types.GOALKEEPER)
                {
                    float character_x = c.transform.position.x;
                    if (character_x > pos)
                        pos = character_x;
                }
            }
        }
        return pos;
    }
    int rep = 0;
    public Character GetOponentFor(Character ch)
    {
        rep = 0;       
        Character oponent =  GetOponentForType(ch, ch.type);
       // Debug.Log("ch " + ch.teamID + " myType: " + ch.type + " fieldPosition: " + ch.fieldPosition + " oponent: " + oponent.teamID + " type: " + oponent.type + " fieldPosition: " + oponent.fieldPosition);
        return oponent;
    }
    Character GetOponentForType(Character ch, Character.types myType)
    {
        rep++;
        List<Character> all;        

        if (ch.teamID == 1) all = team2; else all = team1;
        foreach (Character c in all)
        {
            //if (c.fieldPosition == ch.fieldPosition)
            //{
            if (c.type == Character.types.DEFENSOR && myType == Character.types.DELANTERO && !ThisOponentIsTaken(ch.teamID, c))
                return c;
            else if (c.type == Character.types.DELANTERO && myType == Character.types.DEFENSOR && !ThisOponentIsTaken(ch.teamID, c))
                return c;
            else if (c.type == Character.types.CENTRAL && myType == Character.types.CENTRAL && !ThisOponentIsTaken(ch.teamID, c))
                return c;
            else if (c.type == Character.types.GOALKEEPER && myType == Character.types.GOALKEEPER)
                return c;
            // }   
        }

        if (myType == Character.types.DELANTERO) myType = Character.types.CENTRAL;
        else if(myType == Character.types.DEFENSOR) myType = Character.types.CENTRAL;
        else if(Random.Range(0,10)<5)
            myType = Character.types.DEFENSOR;
        else
            myType = Character.types.DELANTERO;

        if (rep > 10)
        {
            Debug.LogError("no hay oponente para " + ch.type);
            return null;
        }
        return GetOponentForType(ch, myType);
       
    }
    bool ThisOponentIsTaken(int teamID, Character oponent)
    {
        List<Character> all;
        if (teamID == 1) all = team1; else all = team2;
        foreach (Character ch in all)
            if (ch.oponent == oponent)
                return true;
        return false;
    }
}
