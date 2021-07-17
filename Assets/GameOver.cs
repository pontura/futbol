using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public GameObject team1_container;
    public GameObject team2_container;
    public float speed = 2;
    public Animation cameraInGame;
    public CharactersManager charactersManager;
    int audioID;
    List<Character> team_win;

    void Start()
    {
        if (Data.Instance.matchData.score.x > Data.Instance.matchData.score.y)
        {
            cameraInGame.Play("gameOver");
            team_win = charactersManager.team1;
        }            
        else
        {
            charactersManager.referi.actions.LookTo(-1);
            cameraInGame.Play("gameOver2");
            team_win = charactersManager.team2;
        }

        int id = 0;
        Character[] team1 = team1_container.GetComponentsInChildren<Character>();
        Character[] team2 = team2_container.GetComponentsInChildren<Character>();

        foreach (Character ch in team1)
        {
            id++;
            if (id >= Data.Instance.matchData.totalCharacters)
                ch.gameObject.SetActive(false);
        }
        id = 0;
        foreach (Character ch in team2)
        {
            id++;
            if (id >= Data.Instance.matchData.totalCharacters)
                ch.gameObject.SetActive(false);
        }

        charactersManager.Init();
        charactersManager.referi.actions.Pita();

        Events.ChangeVolume("crowd_gol", 0.25f);
       
        Events.OnOutroSound(OnPita, audioID);
        
        Invoke("Delayed", 1);
    }
    void Delayed()
    {
        foreach (Character ch in team_win)
            ch.actions.Goal();
    }
    void OnPita()
    {
        audioID++;
        if(audioID>=3)
            Sigue();
        else
             Events.OnOutroSound(OnPita, audioID);
    }
    void Sigue()
    {
        Done();
        
    }
    void Done()
    {
        Invoke("OnDone", 6);       
    }
    void OnDone()
    {
        Data.Instance.LoadLevel("1_MainMenu");
    }
}
