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
        public string name_abr;
        public int designID;
        public Color color;
        public Color clothColorA;
        public Color clothColorB;
        public Color clothColorC;
        public Color clothColorD;
    }
    public Vector2 selectedTeams;
    public GamePlay gameplay;
    public Vector2 dialoguesTimeToAppear = new Vector2(2, 4);
    public float referiSpeed = 1.1f;
    public float dialoguesDuration = 4;
    public float forceBarSpeed = 3.5f;
    public float timeToSwapCharactersAutomatically = 0.7f;
    public float startingUIForceBar = 0.15f;
    public float input_x_sensibilitty = 4;
    public float scaleFactor = 0.6f;
    public float gkSpeed_sale_x = 2.7f;
    public float gkSpeed_sale_z = 4;

    [Serializable]
    public class GamePlay
    {
        public Character.types characterType;
        public float speed;
        public float speedRun;
        public float speedRunWithBall;
        public float freeze_by_kick;
        public float freeze_by_loseBall;
        public float freeze_by_dashBall;
        public float freeze_dash;
        public float distance_to_dash_ai;
        public float dash_percent_attacking;
        public float dash_percent_defending;
        public float goalKeeperSpeed;

        public float height_to_dominate_ball;
        public float speedWithBall;
        public float speedDash;
        public float gk_FlyingDuration;
        public float gk_FlyingSpeed;
        public float gk_CatchOnAir;
        public float speedRunFade;
        public float distanceToForceCentro;
        public float defenseDelay;
        public float attackDelay;
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
        public float kickCentro;
        public float kickCentroAngle;
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
      //   TextAsset targetFile = Resources.Load<TextAsset>("gameplaySettings");
      //   gameplay = JsonUtility.FromJson<GamePlay>(targetFile.text);
    }
    void Start()
    {
        TextAsset targetFile = Resources.Load<TextAsset>("stats/FulboStars - Easy");
        if(targetFile == null)
            Debug.LogError("No existe: " + "stats/FulboStars - Easy" + " acordate de que sea: .csv");
        
        Data.Instance.spreadsheetLoader.CreateListFromFile(targetFile.text, OnDataLoaded);
    }
    public List<GamePlay> stats_by_character;
    public GamePlay GetStats(Character.types characterType, int teamID)
    {
        if (teamID == 1)
            return gameplay;
        else
            foreach (GamePlay gp in stats_by_character)
                if (gp.characterType == characterType)
                    return gp;
        return null;
    }
    void OnDataLoaded(List<SpreadsheetLoader.Line> all)
    {
        int colID = 0;
        int rowID = 0;
        GamePlay gameplayStats;
        foreach (SpreadsheetLoader.Line line in all)
        {
            foreach (string data in line.data)
            {
                if (rowID == 0) // init all:
                {
                    if (colID > 1)
                    {
                        gameplayStats = new GamePlay();                        
                        switch (data)
                        {
                            case "DEF_DOWN": gameplayStats.characterType = Character.types.DEFENSOR_DOWN; break;
                            case "DEF_UP": gameplayStats.characterType = Character.types.DEFENSOR_UP; break;
                            case "CENTRAL": gameplayStats.characterType = Character.types.CENTRAL; break;
                            case "DEL_DOWN": gameplayStats.characterType = Character.types.DELANTERO_DOWN; break;
                            case "DEL_UP": gameplayStats.characterType = Character.types.DELANTERO_UP; break;
                            default: gameplayStats.characterType = Character.types.GOALKEEPER; break;
                        }
                        stats_by_character.Add(gameplayStats);
                        print(data + " " + gameplayStats.characterType);
                    }
                } 
                else if (colID == 0)
                {
                    for (int a = 0; a < stats_by_character.Count+1; a++)
                    {
                        if (a > 0) // va para cada jugador
                            gameplayStats = stats_by_character[a-1];
                        else // va para los settings generales
                            gameplayStats = gameplay;

                        float value;
                        float.TryParse(line.data[a + 1], out value);

                        if (a > 0) // le suma el default:
                        {
                            float defaultValue;
                            float.TryParse(line.data[1], out defaultValue);
                            if (defaultValue > 0)
                                value += defaultValue;
                        }

                        if (value > 0)
                            value /= 10;
                        
                        AddValue(gameplayStats, data, value);
                    }
                }
                colID++;
            }
            colID = 0;
            rowID++;
        }
    }
    void AddValue(GamePlay gameplayStats, string field, float value)
    {
        switch (field)
        {
            case "speed": gameplayStats.speed = value; break;
            case "speedRun": gameplayStats.speedRun = value; break;
            case "speedRunWithBall": gameplayStats.speedRunWithBall = value; break;
            case "freeze_by_kick": gameplayStats.freeze_by_kick = value; break;
            case "freeze_by_loseBall": gameplayStats.freeze_by_loseBall = value; break;
            case "freeze_by_dashBall": gameplayStats.freeze_by_dashBall = value; break;
            case "freeze_dash": gameplayStats.freeze_dash = value; break;
            case "distance_to_dash_ai": gameplayStats.distance_to_dash_ai = value; break;
            case "dash_percent_attacking": gameplayStats.dash_percent_attacking = value; break;
            case "dash_percent_defending": gameplayStats.dash_percent_defending = value; break;
            case "goalKeeperSpeed": gameplayStats.goalKeeperSpeed = value; break;
            case "height_to_dominate_ball": gameplayStats.height_to_dominate_ball = value; break;
            case "speedWithBall": gameplayStats.speedWithBall = value; break;
            case "speedDash": gameplayStats.speedDash = value; break;
            case "gk_FlyingSpeed": gameplayStats.gk_FlyingSpeed = value; break;
            case "gk_FlyingDuration": gameplayStats.gk_FlyingDuration = value; break;
            case "gk_CatchOnAir": gameplayStats.gk_CatchOnAir = value; break;
            case "speedRunFade": gameplayStats.speedRunFade = value; break;
            case "distanceToForceCentro": gameplayStats.distanceToForceCentro = value; break;
            case "defenseDelay": gameplayStats.defenseDelay = value; break;
            case "attackDelay": gameplayStats.attackDelay = value; break;
            case "kickHard": gameplayStats.kickHard = value; break;
            case "kickHardAngle": gameplayStats.kickHardAngle = value; break;
            case "kickSoft": gameplayStats.kickSoft = value; break;
            case "kickSoftAngle": gameplayStats.kickSoftAngle = value; break;
            case "kickBaloon": gameplayStats.kickBaloon = value; break;
            case "kickBaloonAngle": gameplayStats.kickBaloonAngle = value; break;
            case "kickHead": gameplayStats.kickHead = value; break;
            case "kickHeadAngle": gameplayStats.kickHeadAngle = value; break;
            case "kickChilena": gameplayStats.kickChilena = value; break;
            case "kickChilenaAngle": gameplayStats.kickChilenaAngle = value; break;
            case "kickCentro": gameplayStats.kickCentro = value; break;
            case "kickCentroAngle": gameplayStats.kickCentroAngle = value; break;
        }
    }
}
