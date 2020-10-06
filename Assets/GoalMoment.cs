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
        Events.OnRestartGame += OnRestartGame;
        charactersManager = Game.Instance.charactersManager;
    }
    void OnDestroy()
    {
        Events.OnRestartGame -= OnRestartGame;
    }
    public IEnumerator Init(int teamID, Character character)
    {
        character_made_goal = character;

        if (teamID == 1)
            winners = charactersManager.team1;
        else
            winners = charactersManager.team2;

        Events.OnGoal(teamID, character);
        Game.Instance.cameraInGame.OnGoal(character);
        Events.PlaySound("crowd", "crowd_gol", true);

        yield return new WaitForSeconds(0.2f);
        state = states.GOING_TO_TARGET;        

        if (character.teamID == teamID)
            character_made_goal.actions.Goal();

        yield return new WaitForSeconds(3);
        Events.ChangeVolume("croud", 0.25f);
        state = states.IDLE;
        Events.PlaySound("crowd", "crowd_quiet", true);
        
        yield return new WaitForSeconds(4);
        Events.ChangeVolume("croud", 0.5f);
    }
    void OnRestartGame()
    {
        Game.Instance.ball.Reset();
        Game.Instance.charactersManager.ResetAll();
        Game.Instance.cameraInGame.SetTargetTo(Game.Instance.ball.transform);
        StartCoroutine( Game.Instance.OnWaitToStart() );
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
                Vector3 pos = character.transform.position;
                if (Mathf.Abs(pos.x - targetPos.x) > 0.25f)
                {
                    if (pos.x < targetPos.x) _x = 1;
                    else if (pos.x > targetPos.x) _x = -1;
                }
                if (pos.z < targetPos.z) _z = 1;
                else if (pos.z > targetPos.z) _z = -1;
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
