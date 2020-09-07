using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalMoment : MonoBehaviour
{
    states state;
    public enum states
    {
        IDLE,
        GOING_TO_TARGET,
        WAITING
    }
    Character character_made_goal;
    CharactersManager charactersManager;
    List<Character> winners;

    void Start()
    {
        charactersManager = Game.Instance.charactersManager;
    }
    public IEnumerator Init(int teamID, Character character)
    {
        if (character.teamID == teamID)
            character_made_goal = character;
        else
            character_made_goal = SetGoalMadeCharacter(teamID);

        if (teamID == 1)
            winners = charactersManager.team1;
        else
            winners = charactersManager.team2;

        Events.OnGoal(teamID);
        Game.Instance.cameraInGame.OnGoal(character_made_goal);
        Events.PlaySound("crowd", "crowd_gol");

        yield return new WaitForSeconds(0.2f);
        state = states.GOING_TO_TARGET;
        character_made_goal.actions.Goal();
        yield return new WaitForSeconds(6);
        state = states.IDLE;
        Events.PlaySound("crowd", "crowd_quiet");
        Game.Instance.ball.Reset();
        Game.Instance.charactersManager.ResetAll();
        Game.Instance.cameraInGame.SetTargetTo(Game.Instance.ball.transform);
        Events.OnGameStatusChanged(Game.states.PLAYING);
        winners = null;
    }
    void Update()
    {
        if (state == states.IDLE) return;
      
        foreach (Character character in winners)
        {
            Vector3 targetPos;
            if (character.characterID == character_made_goal.characterID)
                targetPos = new Vector3(character.transform.position.x, 0, 10);
            else
            {
                targetPos = character_made_goal.transform.position - character.ai.aiPosition.originalPosition / 5;
            }

            if (Vector3.Distance(character.transform.position, targetPos) > 3)
            {
                int _x = 0;
                int _z = 0;
                if (character.transform.position.x < targetPos.x) _x = 1;
                else if (character.transform.position.x > targetPos.x) _x = -1;

                if (character.transform.position.z < targetPos.z) _z = 1;
                else if (character.transform.position.z > targetPos.z) _z = -1;
                character.MoveTo(_x, _z);
            }
            else if(character.actions.state != CharacterActions.states.GOAL)
            {
                character.actions.Goal();
            }
        }
    }
    Character SetGoalMadeCharacter(int teamID)
    {
        if (teamID == 1)
            return charactersManager.team1[Random.Range(0, charactersManager.team1.Count-2)];
        else
            return charactersManager.team2[Random.Range(0, charactersManager.team2.Count - 2)];
    }
}
