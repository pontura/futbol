using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionsUI : MonoBehaviour
{
    public PositionsUIManager team1_manager;
    public PositionsUIManager team2_manager;
    public int positionID_team1;
    public int positionID_team2;

    void Start()
    {
        Events.OnRight += OnRight;
        Events.OnButtonClick += OnButtonClick;
        positionID_team1 = 0;
        positionID_team2 = 1;

        Init(1);
        Init(2);
    }
    private void OnDestroy()
    {
        Events.OnRight -= OnRight;
        Events.OnButtonClick -= OnButtonClick;
    }
    void OnButtonClick(int a, int b)
    {
        Go();
    }
    void Init(int teamID)
    {
        if(teamID == 1)
            team1_manager.Init(Data.Instance.charactersPositions.all.all[positionID_team1], CharactersData.Instance.team1);
        else
            team2_manager.Init(Data.Instance.charactersPositions.all.all[positionID_team2], CharactersData.Instance.team2);
    }
    void OnRight(int teamID, bool isRight)
    {
        if (teamID == 1)
        {
            if (isRight)
                positionID_team1++;
            else
                positionID_team1--;
            if (positionID_team1 > Data.Instance.charactersPositions.all.all.Length - 1)
                positionID_team1 = 0;
            else if (positionID_team1 < 0)
                positionID_team1 = Data.Instance.charactersPositions.all.all.Length - 1;
            Init(1);
        }
        else if (teamID == 2)
        {
            if (isRight)
                positionID_team2++;
            else
                positionID_team2--;
            if (positionID_team2 > Data.Instance.charactersPositions.all.all.Length - 1)
                positionID_team2 = 0;
            else if (positionID_team2 < 0)
                positionID_team2 = Data.Instance.charactersPositions.all.all.Length - 1;
            Init(2);
        }
    }
    void Go()
    {
        Data.Instance.matchData.SetCharactersPositions(positionID_team1, positionID_team2);
        Data.Instance.LoadLevel("GameIntro");
    }
}
