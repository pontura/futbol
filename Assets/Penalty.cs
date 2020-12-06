 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penalty : MonoBehaviour
{
    bool ataja;
    states state;
    enum states
    {
        WAITING,
        IDLE,
        SHOOT
    }
    public GameObject ballAimer;
    public InputManagerUI inputManager;
    public Character character;
    public Character goalKeeper;
    float goalkeeperSpeed = 2;
    Vector2 goalKeeperDirection;
    Vector2 characterDirection;
    public Animation ballAnim;
    public CharactersManager charactersManager;

    void Start()
    {
        Time.timeScale = 1;
        Events.PlaySound("crowd", "crowd_quiet", true);
        

        Events.OnButtonPressed += OnButtonPressed;
        Events.OnButtonClick += OnButtonClick;
        Events.OnRestartGame += OnRestartGame;
       // goalKeeper.isGoalKeeper = false;
        
    }
    public void PenaltyPita()
    {
        if (Data.Instance.matchData.totalPlayers == 1 && Data.Instance.matchData.penaltyGoalKeeperTeamID == 1)
        {
            //atajas y te patea la compu
            Invoke("KickAutomatic", Random.Range(1.8f, 3.5f));
        }
        state = states.IDLE;
        charactersManager.referi.actions.Pita();
    }
    void OnDestroy()
    {
        Events.OnButtonPressed -= OnButtonPressed;
        Events.OnRestartGame -= OnRestartGame;
        Events.OnButtonClick -= OnButtonClick;
    }
    void OnRestartGame()
    {
        Invoke("GotoGame", 1);
    }
    void GotoGame()
    {
        Data.Instance.LoadLevel("Game");
    }
    void OnButtonClick(int playerID, int a)
    {
        OnButtonPressed(playerID, InputManagerUI.buttonTypes.BUTTON_1);
    }
    void OnButtonPressed(int playerID, InputManagerUI.buttonTypes type)
    {
        if (state == states.IDLE)
        {
            if ((playerID == 1 && Data.Instance.matchData.penaltyGoalKeeperTeamID == 2)
                || (playerID == 2 && Data.Instance.matchData.penaltyGoalKeeperTeamID == 1))
            {
                if (Data.Instance.matchData.totalPlayers == 1)
                    AutomaticGoalKeeperReaction();

                StartCoroutine(Kick());
                
            }
        }
    }
    void Update()
    {
        if (state == states.WAITING)
            return;

        if (state == states.SHOOT)
        {
            UpdateGoalKeeperPos();
            return;
        }

        if (state != states.IDLE)
            return;

        Vector3 pos = goalKeeper.transform.localPosition;

        if (Data.Instance.matchData.penaltyGoalKeeperTeamID == 2)
        {
            goalKeeperDirection = new Vector2((float)inputManager.horizontalAxis_team_2, (float)inputManager.verticalAxis_team_2);
            characterDirection = new Vector2((float)inputManager.horizontalAxis_team_1, (float)inputManager.verticalAxis_team_1);
        } 
        else
        {
            goalKeeperDirection = new Vector2((float)inputManager.horizontalAxis_team_1, (float)inputManager.verticalAxis_team_1);
            characterDirection = new Vector2((float)inputManager.horizontalAxis_team_2, (float)inputManager.verticalAxis_team_2);
        }

        pos.x += goalkeeperSpeed * Time.deltaTime * goalKeeperDirection.x;
        pos.z += (goalkeeperSpeed/2) * Time.deltaTime * inputManager.verticalAxis_team_2;
        if (pos.z < 8.7f) pos.z = 8.7f; else if (pos.z > 9.6f) pos.z = 9.6f;
        float _x_max = 0.2f;
        if (pos.x < -_x_max) pos.x = -_x_max; else if (pos.x > _x_max) pos.x = _x_max;
       // goalKeeper.transform.localPosition = pos;

        if (goalKeeperDirection.x != 0 || inputManager.verticalAxis_team_2 != 0)
        {
            //goalKeeper.actions.Run();
            //if (goalKeeperDirection.x > 0)         goalKeeper.actions.LookTo(1);
            //else if (goalKeeperDirection.x < 0)    goalKeeper.actions.LookTo(-1);
        }
        else
            goalKeeper.actions.Idle();

        Aim(characterDirection.x);
    }
    void Aim(float dir)
    {
        characterDirection.x = dir;
        ballAimer.transform.localEulerAngles = new Vector3(0, dir * 24, 0);
        ballAnim.transform.localEulerAngles = new Vector3(0, dir * 24, 0);
    }
    IEnumerator Kick()
    {
        if (goalKeeperDirection.x > 0)         goalKeeper.actions.LookTo(1);
        else if (goalKeeperDirection.x < 0)    goalKeeper.actions.LookTo(-1);
        state = states.SHOOT;
        character.actions.Kick(CharacterActions.kickTypes.HARD);
        goalKeeper.actions.GoalKeeperJumpType((int)goalKeeperDirection.x, false);

        print(characterDirection.x + " " + goalKeeperDirection.x);

        if (characterDirection.x == goalKeeperDirection.x)
            ataja = true;

        if(ataja)
            ballAnim.Play("penaltyBallAtaja");
        else
            ballAnim.Play("penaltyBall");
        
        yield return new WaitForSeconds(0.1f);
        Events.PlaySound("crowd", "crowd_gol", true);

        if (ataja)
            Events.OnGoal(charactersManager.teamID_2, goalKeeper);
        else
        {
            Data.Instance.matchData.OnGoal(charactersManager.teamID_1);
            Events.OnGoal(charactersManager.teamID_1, character);
        }

        yield return new WaitForSeconds(0.4f);
        if (ataja)
            goalKeeper.actions.Goal();
        else
            character.actions.Goal();

    }
    void UpdateGoalKeeperPos()
    {
        if (goalKeeperDirection.x == 0)
            return;
        Vector3 dest = goalKeeper.transform.localPosition;
        dest.x = 2 * goalKeeperDirection.x;
        goalKeeper.transform.localPosition = Vector3.Lerp(goalKeeper.transform.localPosition, dest, 10*Time.deltaTime);
    }
    void KickAutomatic()
    {
        float dir = 0;
        float rand = Random.Range(0, 100);
        if (rand < 33)
            dir = -1;
        else if (rand < 66)
            dir = 1;
        Aim(dir);
        StartCoroutine(Kick());
    }
    void AutomaticGoalKeeperReaction()
    {
        float dir = 0;
        float rand = Random.Range(0, 100);
        if (rand < 33)
            dir = -1;
        else if (rand < 66)
            dir = 1;
        goalKeeperDirection.x = dir;
    }
}
