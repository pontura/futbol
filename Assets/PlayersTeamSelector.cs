using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersTeamSelector : MonoBehaviour
{
    public PlayerTeamSignalUI[] all;
    public Image team1Escudo;
    public Image team2Escudo;
    public Text field;

    void Start()
    {
        all = GetComponentsInChildren<PlayerTeamSignalUI>();
        int id = 1;
        foreach (PlayerTeamSignalUI p in all)
        {
            p.Init(id);
            id++;
        }
        Events.OnRight += OnRight;
        Events.OnSkipOn(Go);

        int team1 = (int)Data.Instance.settings.selectedTeams[0];
        int team2 = (int)Data.Instance.settings.selectedTeams[1];

        team1Escudo.sprite = Data.Instance.settings.teamSettings[team1].escudo;
        team2Escudo.sprite = Data.Instance.settings.teamSettings[team2].escudo;
    }
    private void OnDestroy()
    {
        Events.OnRight -= OnRight;
    }
    void OnRight(int playerID, bool isRight)
    {
        int id = playerID - 1;
        all[id].MoveRight(isRight);
        SetTotalPlayers();
    }
    void Go()
    {
        Events.OnSkipOff();
        if (AnyActive()) {            
            Data.Instance.LoadLevel("Positions");
        }
        else
        {
            print("VA");
            Events.OnSkipOn(Go);
        }
    }
    bool AnyActive()
    {
        bool oneActive = false;
        int id = 0;
        foreach (PlayerTeamSignalUI p in all)
        {
            if (p.teamID != 0)
                oneActive = true;
            Data.Instance.matchData.AddPlayer(id + 1, p.teamID);
            id++;
        }
        if (oneActive)
            return true;
        return false;
    }
    void SetTotalPlayers()
    {
        int total = 0;
        foreach (PlayerTeamSignalUI p in all)
        {
            if (p.teamID != 0)
                total++;
        }
        field.text = total + " JUGADORES";
    }
}
