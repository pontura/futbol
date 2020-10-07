 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penalty : MonoBehaviour
{
    bool ataja;
    states state;
    enum states
    {
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
        Events.OnRestartGame += OnRestartGame;
        goalKeeper.isGoldKeeper = false;
    }
    void OnDestroy()
    {
        Events.OnButtonPressed -= OnButtonPressed;
        Events.OnRestartGame -= OnRestartGame;
    }
    void OnRestartGame()
    {
        Invoke("GotoGame", 1);
    }
    void GotoGame()
    {
        Data.Instance.LoadLevel("Game");
    }
    void OnButtonPressed(int playerID, InputManagerUI.buttonTypes type)
    {
        if (state == states.IDLE)
        {
            if (playerID == 1 && Data.Instance.matchData.penaltyGoalKeeperTeamID == 2)            
                StartCoroutine(Kick());
            else if (playerID == 2 && Data.Instance.matchData.penaltyGoalKeeperTeamID == 1)
                StartCoroutine(Kick());
        }
    }
    void Update()
    {
        if (state == states.SHOOT)
            UpdateGoalKeeperPos();

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
        if (pos.x < -2.2f) pos.x = -2.2f; else if (pos.x > 2.2f) pos.x = 2.2f;
        goalKeeper.transform.localPosition = pos;

        if (goalKeeperDirection.x != 0 || inputManager.verticalAxis_team_2 != 0)
        {
            goalKeeper.actions.Run();
            if (goalKeeperDirection.x > 0)         goalKeeper.actions.LookTo(1);
            else if (goalKeeperDirection.x < 0)    goalKeeper.actions.LookTo(-1);
        }
        else
            goalKeeper.actions.Idle();

        ballAimer.transform.localEulerAngles = new Vector3(0, characterDirection.x * 24, 0);
        ballAnim.transform.localEulerAngles = new Vector3(0, characterDirection.x * 24, 0);
    }
    IEnumerator Kick()
    {
        state = states.SHOOT;
        character.actions.Kick(CharacterActions.kickTypes.HARD);
        goalKeeper.actions.GoalKeeperJumpType((int)goalKeeperDirection.x, false);

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
}
