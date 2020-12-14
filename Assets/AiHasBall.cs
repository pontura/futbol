using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiHasBall : MonoBehaviour
{
    AI ai;
    Vector3 dest;
    public Character characterToPass;
    float center_goto_goal_x = 15;
    int _z = 0;
    public float timer;

    private void Awake()
    {
        ai = GetComponent<AI>();
    }
    private void Start()
    {
        Reset();
    }
    public void SetActive()
    {
        characterToPass = null;
        timer = Time.time;
        dest.x = center_goto_goal_x;
        if (ai.character.teamID == 1)
            dest.x *= -1;
        enabled = true;
        Invoke("Loop", 0.35f);
    }
    void Loop()
    {
        if (ai.character.isBeingControlled)
            return;

        Invoke("Loop", 0.75f);

        if (characterToPass == null && timer + 1 > Time.time && Random.Range(0,10)<4)
            GiveBall();
        
        int rand = Random.Range(0, 100);
        if (rand < 30)
            _z = 1;
        else if (rand < 60)
            _z = -1;
        else
            _z = 0;

        if(rand<70)
            ai.character.SuperRun();
    }
    public void Reset()
    {
        CancelInvoke();
        enabled = false;
        characterToPass = null;
    }
    void KickBall()
    {
        ai.character.Kick(CharacterActions.kickTypes.KICK_TO_GOAL);
    }
    public virtual void UpdatedByAI()
    {
        if (ai.character.isBeingControlled)
            return;
        if (characterToPass == null && Mathf.Abs(transform.position.x - dest.x) < 1)
        {
            KickBall();
            return;
        }
        int _x = 0;

        if (transform.position.x < dest.x) _x = 1; else _x = -1;
        if (Mathf.Abs(transform.position.z- dest.z) > 3f)
        {
            if (transform.position.z < dest.z) _z = 1; else _z = -1;
        }
        if (_x == 0 && _z == 0)
            return;
        if (ai == null || ai.character == null)
            return;
        ai.character.MoveTo(_x, _z);
        timer += Time.deltaTime;
    }
    void GiveBall()
    {
        characterToPass = Game.Instance.charactersManager.GetNearestTo(ai.character, ai.character.teamID);

        Vector3 otherPos = characterToPass.transform.position;
        float offset = 3;
        if (ai.character.teamID == 1)
            otherPos.x -= offset;
        else if (ai.character.teamID == 2)
            otherPos.x += offset;

        ai.character.ballCatcher.LookAt(otherPos);
        ai.character.Kick(CharacterActions.kickTypes.SOFT, (float)Random.Range(10, 30) / 10);
        Reset();
    }
}
