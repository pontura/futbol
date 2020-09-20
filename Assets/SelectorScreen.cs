using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorScreen : MonoBehaviour
{
    public Ruleta ruletaEscudo_team1;
    public Ruleta ruletaEscudo_team2;
    public Text team1_field;
    public Text team2_field;
    public states state;
    public GameObject characters1_container;
    public GameObject characters2_container;
    Ruleta[] character1;
    Ruleta[] character2;

    public Text[] character1_texts;
    public Text[] character2_texts;
    public int teamsDone;
    public enum states
    {
        IDLE,
        TEAM,
        PLAYERS,
        DONE
    }
    List<Sprite> all;
    void Start()
    {
        Data.Instance.charactersData.Init();
        Events.OnButtonPressed += OnButtonPressed;
      
        character1 = characters1_container.GetComponentsInChildren<Ruleta>();
        character2 = characters2_container.GetComponentsInChildren<Ruleta>();

        character1_texts = characters1_container.GetComponentsInChildren<Text>();
        character2_texts = characters2_container.GetComponentsInChildren<Text>();

        all = new List<Sprite>();
        foreach (Settings.TeamSettings data in Data.Instance.settings.teamSettings) all.Add(data.escudo);

    }
    private void Init()
    {
        teamsDone = 0;
      
        ruletaEscudo_team1.Init(all);
        ruletaEscudo_team2.Init(all);
    }
    void OnDestroy()
    {
        Events.OnButtonPressed -= OnButtonPressed;
    }
    public void ButtonClicked()
    {
        if(state == states.IDLE)
        {
            Init();
            ruletaEscudo_team1.SetOn(OnDoneTeam);
            ruletaEscudo_team2.SetOn(OnDoneTeam);
            state = states.TEAM;
        }
    }
    void OnButtonPressed(int playerID, InputManagerUI.buttonTypes type)
    {
        ButtonClicked();
    }
   
    void OnDoneTeam(int selectedID)
    {        
        teamsDone++;
        if(teamsDone>=2)
        {
            if (ruletaEscudo_team1.selectedID == ruletaEscudo_team2.selectedID)
            {
                state = states.IDLE;
                ruletaEscudo_team1.state = ruletaEscudo_team2.state = Ruleta.states.IDLE;
                teamsDone = 0;
                ButtonClicked();
            }
            else
            {
                TeamsDone();
            }
        }
    }
    void TeamsDone()
    {
        Data.Instance.settings.selectedTeams = new Vector2(ruletaEscudo_team1.selectedID, ruletaEscudo_team2.selectedID);
        team1_field.text = Data.Instance.settings.teamSettings[ruletaEscudo_team1.selectedID].name;
        team2_field.text = Data.Instance.settings.teamSettings[ruletaEscudo_team2.selectedID].name;
        SetCharacter(1, false);
        SetCharacter(2, false);
    }
    public int team1_characterID;
    public int team2_characterID;
    void SetCharacter(int teamID, bool isGoalKeeper)
    {
        print("teamID: " + teamID + "  isGoalKeeper: " + isGoalKeeper + "   team2_characterID: " + team2_characterID + "    team1_characterID: " + team1_characterID);
        Ruleta ruleta;
        List<Sprite> all = Data.Instance.charactersData.GetAvailablePlayers(teamID, isGoalKeeper);
        if (teamID == 1)
        {
            if (team1_characterID >= 5)
            {
                TeamReady();
                return;
            }
            ruleta = character1[team1_characterID];
            ruleta.Init(all);
            ruleta.SetOn(OnCharacterDoneTeam1);
            
        }
        else
        {
            if (team2_characterID >= 5)
            {
                TeamReady();
                return;
            }
            ruleta = character2[team2_characterID];
            ruleta.Init(all);
            ruleta.SetOn(OnCharacterDoneTeam2);
            
        }
    }
    void OnCharacterDoneTeam1(int id)
    {
        int characterID;
        team1_characterID++;
        if (team1_characterID >= 4)
        {
            print("goalkeeper team1_characterID " + team1_characterID + "  id " + id);
            characterID = Data.Instance.charactersData.availablesTeam1_goalkeepers[0];
          //  print("team1_characterID " + team1_characterID + " id: " + id + " characterID: " + characterID);
            character1_texts[team1_characterID-1].text = Data.Instance.textsData.GetCharactersData(characterID, true).avatarName;

            Data.Instance.charactersData.AddCharacterToTeam(1, characterID);
            SetCharacter(1, true);
        }
        else
        {
            characterID = Data.Instance.charactersData.availablesTeam1[id];
            character1_texts[team1_characterID-1].text = Data.Instance.textsData.GetCharactersData(characterID).avatarName;

            Data.Instance.charactersData.AddCharacterToTeam(1, characterID);
            SetCharacter(1, false);
        }

    }
    void OnCharacterDoneTeam2(int id)
    {
        int characterID;
        team2_characterID++;
        if (team2_characterID >= 4)
        {
            print("goalkeeper team2_characterID " + team2_characterID + "  id " + id);
            print("id " + id);
            characterID = Data.Instance.charactersData.availablesTeam2_goalkeepers[0];
           // print("team2_characterID " + team2_characterID + " id: " + id + " characterID: " + characterID);
            character2_texts[team2_characterID-1].text = Data.Instance.textsData.GetCharactersData(characterID, true).avatarName;

            Data.Instance.charactersData.AddCharacterToTeam(2, characterID);
              SetCharacter(2, true);
        }
        else
        {
            characterID = Data.Instance.charactersData.availablesTeam2[id];
            character2_texts[team2_characterID-1].text = Data.Instance.textsData.GetCharactersData(characterID).avatarName;

            Data.Instance.charactersData.AddCharacterToTeam(2, characterID);
            SetCharacter(2, false);
        }

        
    }
    int teamsReady = 0;
    void TeamReady()
    {
        teamsReady++;
        if(teamsReady>=2)
        {
            state = states.DONE;
            Invoke("Go", 3);
        }
            
    }
    void Go()
    {
        Data.Instance.LoadLevel("Game");
    }
}
