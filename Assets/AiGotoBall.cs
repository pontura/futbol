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
        if (distToBall<3 && Random.Range(0,10)<5)
        {
            ai.character.Dash();
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
        if (Mathf.Abs(transform.position.x - dest.x) > 0.25f)
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
