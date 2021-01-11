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
    public Ruleta referiRuleta;
    public Text referiName;

    public GameObject characterRuleta_to_instantiate;

    Ruleta[] character1;
    Ruleta[] character2;    

    public Text[] character1_texts;
    public Text[] character2_texts;
    public int teamsDone;
    int totalPlayers;

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
        CharactersData.Instance.Init();
        Events.OnButtonClick += OnButtonClick;

        for(int a = 0; a<Data.Instance.stadiumData.active.totalPlayers; a++)
        {
            GameObject go;
            go = Instantiate(characterRuleta_to_instantiate, characters1_container.transform);
            go = Instantiate(characterRuleta_to_instantiate, characters2_container.transform);
        }
        

        character1 = characters1_container.GetComponentsInChildren<Ruleta>();
        character2 = characters2_container.GetComponentsInChildren<Ruleta>();

        character1_texts = characters1_container.GetComponentsInChildren<Text>();
        character2_texts = characters2_container.GetComponentsInChildren<Text>();

        totalPlayers = character1_texts.Length;

        all = new List<Sprite>();
        foreach (Settings.TeamSettings data in Data.Instance.settings.teamSettings) all.Add(data.escudo);

        if (Data.Instance.isMobile)
        {
            Invoke("ButtonClicked", 1);
        }
    }
    private void Init()
    {
        teamsDone = 0;
      
        ruletaEscudo_team1.Init(all);
        ruletaEscudo_team2.Init(all);
    }
    void OnDestroy()
    {
        Events.OnButtonClick -= OnButtonClick;
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
    void OnButtonClick(int playerID, int id)
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
        Ruleta ruleta;
        List<Sprite> all = CharactersData.Instance.GetAvailablePlayers(teamID, isGoalKeeper);
        if (teamID == 1)
        {
            if (team1_characterID >= totalPlayers)
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
            if (team2_characterID >= totalPlayers)
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
        if (team1_characterID >= totalPlayers)
        {
            characterID = CharactersData.Instance.availablesTeam1_goalkeepers[id];
            character1_texts[team1_characterID-1].text = Data.Instance.textsData.GetCharactersData(characterID, true).avatarName;

            CharactersData.Instance.AddCharacterToTeam(1, characterID);
            TeamReady();
        }
        else
        {
            characterID = CharactersData.Instance.availablesTeam1[id];
            character1_texts[team1_characterID-1].text = Data.Instance.textsData.GetCharactersData(characterID).avatarName;

            CharactersData.Instance.AddCharacterToTeam(1, characterID);
            if(team1_characterID == totalPlayers-1)
                SetCharacter(1, true);
            else
                SetCharacter(1, false);
        }

    }
    void OnCharacterDoneTeam2(int id)
    {
        int characterID;
        team2_characterID++;
        if (team2_characterID >= totalPlayers)
        {
            characterID = CharactersData.Instance.availablesTeam2_goalkeepers[id];
            character2_texts[team2_characterID-1].text = Data.Instance.textsData.GetCharactersData(characterID, true).avatarName;

            CharactersData.Instance.AddCharacterToTeam(2, characterID);
            TeamReady();
        }
        else
        {
            characterID = CharactersData.Instance.availablesTeam2[id];
            character2_texts[team2_characterID-1].text = Data.Instance.textsData.GetCharactersData(characterID).avatarName;

            CharactersData.Instance.AddCharacterToTeam(2, characterID);
            if (team2_characterID == totalPlayers - 1)
                SetCharacter(2, true);
            else
                SetCharacter(2, false);
        }

        
    }
    int teamsReady = 0;
    void TeamReady()
    {
        teamsReady++;
        if(teamsReady>=2)
        {
            SetReferi();
        }
            
    }
    void Go()
    {
        Data.Instance.LoadLevel("GameIntro");
    }
    void SetReferi()
    {
        List<Sprite> all = CharactersData.Instance.GetReferies();
        referiRuleta.Init(all);
        referiRuleta.SetOn(OnReferiDone);
    }
    void OnReferiDone(int id)
    {
        CharactersData.Instance.referiId = CharactersData.Instance.availableReferis[id];
        referiName.text = Data.Instance.textsData.GetReferisData(CharactersData.Instance.referiId).avatarName;
        AllLoaded();
    }
    void AllLoaded()
    {
        state = states.DONE;
        Invoke("Go", 3);
    }
}
