using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
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

        Events.ChangeVolume("crowd_gol", 0.25f);
        charactersManager.Init(0);
        charactersManager.referi.actions.Pita();
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
        Events.OnGameOverVoiceHappy(team_win[Random.Range(0,5)], Done);
    }
    void Done()
    {
        Invoke("OnDone", 11);       
    }
    void OnDone()
    {
        Data.Instance.LoadLevel("1_MainMenu");
    }
}
