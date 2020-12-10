using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Settings : MonoBehaviour
{
    public int totalPlayers = 1;
    public int totalTime;
    public TeamSettings[] teamSettings;
    [Serializable]
    public class TeamSettings
    {
        public Sprite escudo;
        public string name;
        public int designID;
        public Color color;
        public Color clothColorA;
        public Color clothColorB;
        public Color clothColorC;
        public Color clothColorD;
    }
    public Vector2 selectedTeams;
    public GamePlay gameplay;

    [Serializable]
    public class GamePlay
    {
        public Vector2 limits = new Vector2(40, 20);
        public float input_x_sensibilitty;
        public float scaleFactor;
        public float speed;
        public float speedRun;
        public float speedRunWithBall;
        public float freeze_by_loseBall;
        public float freeze_by_dashBall;
        public float freeze_dash;
        public float goalKeeperSpeed;
        public float gkSpeed_sale_x;
        public float gkSpeed_sale_z;
        public float referiSpeed;
        public float speedWithBall;
        public float speedDash;
        public Vector2 dialoguesTimeToAppear = new Vector2(2, 4);
        public float dialoguesDuration;
        public float timeToSwapCharactersAutomatically = 0.7f;        
        public float forceBarSpeed;
        public int gk_CatchOnAir;
        public float speedRunFade;
        public float distanceToForceCentro;
        public float startingUIForceBar;

        public float kickHard;
        public float kickHardAngle;

        public float kickSoft;
        public float kickSoftAngle;

        public float kickBaloon;
        public float kickBaloonAngle;

        public float kickHead;
        public float kickHeadAngle;

        public float kickChilena;
        public float kickChilenaAngle;      
    }   

    public TeamSettings GetTeamSettings(int teamID)
    {
        if (teamID == 1)
            return teamSettings[(int)Data.Instance.settings.selectedTeams.x];
        else
            return teamSettings[(int)Data.Instance.settings.selectedTeams.y];
    }

    void Awake()
    {
        TextAsset targetFile = Resources.Load<TextAsset>("gameplaySettings");
        gameplay = JsonUtility.FromJson<GamePlay>(targetFile.text);
    }
}
