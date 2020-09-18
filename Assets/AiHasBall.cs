using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiHasBall : MonoBehaviour
{
    AI ai;
    Vector3 dest;
    float center_goto_goal_x = 15;

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
        dest.x = center_goto_goal_x;
        if (ai.character.teamID == 1)
            dest.x *= -1;
        enabled = true;
        Invoke("Loop", 0.25f);

        if (ai.character.characterID == 8)
            print(dest);
    }
    void Loop()
    {       
        Invoke("Loop", 1f);
    }
    public void Reset()
    {
        CancelInvoke();
        enabled = false;
    }
    void KickBall()
    {
        ai.character.Kick(CharacterActions.kickTypes.KICK_TO_GOAL);
    }
    public virtual void UpdatedByAI()
    {
        if (Mathf.Abs(transform.position.x - dest.x) < 1)
        {
            KickBall();
            return;
        }
        int _x = 0;
        int _z = 0;
        
        if (transform.position.x < dest.x) _x = 1; else _x = -1;
        if (transform.position.z < dest.z) _z = 1; else _z = -1;
        if (_x == 0 && _z == 0)
            return;
        if (ai == null || ai.character == null)
            return;
        ai.character.MoveTo(_x, _z);
    }
}
