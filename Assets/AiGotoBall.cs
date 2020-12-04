using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiGotoBall : MonoBehaviour
{
    AI ai;
    Vector3 dest;
    private void Start()
    {
        ai = GetComponent<AI>();
        Reset();
    }
    public void SetActive()
    {
        enabled = true;
        Invoke("Loop", 0.25f);
    }
    void Loop()
    {
        dest = ai.ball.transform.position;
        float distToBall =  Vector3.Distance(transform.position, dest);
        if (distToBall > 20)
        {
            ai.character.SuperRun();
        }       
        else if (distToBall<4 && Random.Range(0,10)<7)
        {
            ai.character.Dash();
        }
        else if (
            ai.character.teamID == 1 && ai.character.transform.position.x > dest.x
           || ai.character.teamID == 2 && ai.character.transform.position.x < dest.x)
        {
            ai.character.SuperRun();
        }
        Invoke("Loop", 0.85f);
    }
    public void Reset()
    {
        CancelInvoke();
        enabled = false;
    }
    public virtual void UpdatedByAI()
    {
        int _x = 0;
        int _z = 0;
        if (Mathf.Abs(transform.position.x - dest.x) > 0.15f)
        {
            if (transform.position.x < dest.x) _x = 1; else _x = -1;
        }
        if (transform.position.z < dest.z)  _z = 1;  else _z = -1;
        if (_x == 0 && _z == 0)
            return;
        if (ai == null || ai.character == null)
            return;
            ai.character.MoveTo(_x, _z);
    }
}
