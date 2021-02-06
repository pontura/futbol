using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalMoment : MonoBehaviour
{
    public states state;
    public enum states
    {
        IDLE,
        GOING_TO_TARGET
    }
    Character character_made_goal;
    CharactersManager charactersManager;
    List<Character> winners;

    void Start()
    {
        charactersManager = Game.Instance.charactersManager;
    }
    public void Init(int teamID, Character character)
    {
        StartCoroutine(InitC(teamID, character));
    }
    IEnumerator InitC(int teamID, Character character)
    {        
        character_made_goal = character;

        if (teamID == 1)
            winners = charactersManager.team1;
        else
            winners = charactersManager.team2;

        Events.ChangeVolume("croud", 1);
        Data.Instance.matchData.OnGoal(teamID);
        Events.OnGoal(teamID, character);
        Game.Instance.cameraInGame.OnGoal(character);
        Events.PlaySound("crowd", "crowd_gol", true);

        yield return new WaitForSeconds(0.2f);
        state = states.GOING_TO_TARGET;        

        if (character.teamID == teamID)
            character_made_goal.actions.Goal();
        yield return new WaitForSeconds(0.3f);
        Events.OnSkipOn(Done);
        yield return new WaitForSeconds(3.7f);        
        state = states.IDLE;
        yield return new WaitForSeconds(1);

        foreach (Character ch in winners)
            ch.actions.Goal();

        yield return new WaitForSeconds(2);
        Events.ChangeVolume("croud", 0.5f);
        
    }
    public void Done()
    {
        if (Game.Instance.state != Game.states.GOAL)
            return;

        StopAllCoroutines();
        Events.OnSkipOff();
        Game.Instance.ball.Reset();
        Game.Instance.charactersManager.ResetAll();
        state = states.IDLE;
        StartCoroutine( Game.Instance.OnWaitToStart() );
        winners = null;

        UIMain.Instance.OnShow();
    }
    void Update()
    {
        if (state == states.IDLE) return;

        foreach (Character character in winners)
        {
            Vector3 targetPos;
            if (character.data.id == character_made_goal.data.id)
                targetPos = new Vector3(character.transform.position.x, 0, 10);
            else
            {
                targetPos = character_made_goal.transform.position - character.ai.originalPosition / 5;
            }

            if (Vector3.Distance(character.transform.position, targetPos) > 3)
            {
                int _x = 0;
                int _z = 0;
                Vector3 pos = character.transform.position;
                if (Mathf.Abs(pos.x - targetPos.x) > 0.25f)
                {
                    if (pos.x < targetPos.x) _x = 1;
                    else if (pos.x > targetPos.x) _x = -1;
                }
                if (pos.z < targetPos.z) _z = 1;
                else if (pos.z > targetPos.z) _z = -1;
                character.MoveTo(_x, _z);
            } else
                character.actions.Goal();
        }
    }
}
