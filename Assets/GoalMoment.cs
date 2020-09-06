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
        this.character_made_goal = character;
        if (teamID == 1)
            winners = charactersManager.team1;
        else
            winners = charactersManager.team2;

        Events.OnGoal(teamID);
        Game.Instance.cameraInGame.OnGoal(character);
        Events.PlaySound("crowd", "crowd_gol");

        yield return new WaitForSeconds(0.2f);
        state = states.GOING_TO_TARGET;
        character.actions.Goal();
        yield return new WaitForSeconds(4);

        Events.PlaySound("crowd", "crowd_quiet");
        Game.Instance.ball.Reset();
        Game.Instance.charactersManager.ResetAll();
        Game.Instance.cameraInGame.SetTargetTo(Game.Instance.ball.transform);
        Events.OnGameStatusChanged(Game.states.PLAYING);
        
    }
    void Update()
    {
        if (state == states.IDLE) return;
      
        foreach (Character character in winners)
        {
            Vector3 targetPos;
            if (character.characterID == character_made_goal.characterID)
                targetPos = new Vector3(0, 0, 10);
            else
                targetPos = character_made_goal.transform.position;

            int _x = 0;
            int _z = 0;

            if (character.transform.position.x < targetPos.x) _x = 1;
            else if (character.transform.position.x > targetPos.x) _x = -1;

            if (character.transform.position.z < targetPos.z) _z = 1;
            else if (character.transform.position.z > targetPos.z) _z = -1;
            character.MoveTo(_x, _z);
        }
    }
}
