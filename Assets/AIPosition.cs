using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPosition : MonoBehaviour
{
    public Vector3 originalPosition;
    Vector3 gotoPosition;
    public AI ai;
    public states state;
    public enum states
    {
        GOING,
        WAITING
    }

    void Start()
    {
        ai = GetComponent<AI>();
        originalPosition = transform.position;
        //enabled = false;
    }
    public virtual void UpdatedByAI()
    {      
        int _h, _v = 0;
        if (Vector3.Distance(transform.position, gotoPosition) > 0.5f)
        {
            if (transform.position.x < gotoPosition.x)
                _h = 1;
            else
                _h = -1;
            if (transform.position.z < gotoPosition.z)
                _v = 1;
            else
                _v = -1;
            ai.character.SetPosition(_h, _v);
        } else if (state == states.GOING)
        {
            ai.character.actions.Idle();
            state = states.WAITING;
            Invoke("StopWaiting", GetRandomBetween(0,40) );
        }
    }
    public virtual void SetActive()
    {
        this.enabled = true;
        state = states.GOING;
        if (ai.state == AI.states.DEFENDING)
            gotoPosition = originalPosition;
        else
        {
            gotoPosition = originalPosition;
            if(ai.character.teamID == 1)
                gotoPosition.x = originalPosition.x - (Data.Instance.settings.limits.x / 2) + ((float)Random.Range(0,30)/10);
            else
                gotoPosition.x = originalPosition.x + (Data.Instance.settings.limits.x / 2) - ((float)Random.Range(0, 30)/10);

            gotoPosition.z += ((float)Random.Range(-30, 30) / 10);
            SetLimits();
        }
    }
    void StopWaiting()
    {
        state = states.GOING;
        float _x = GetRandomBetween(-40, 40);
        float _z = GetRandomBetween(-40, 40);
        gotoPosition = transform.position + new Vector3(_x, 0, _z);
        SetLimits();
    }
    void SetLimits()
    {
        if (Mathf.Abs(gotoPosition.z) > Data.Instance.settings.limits.y / 2)
            gotoPosition.z = originalPosition.z;
        if (Mathf.Abs(gotoPosition.x) > Data.Instance.settings.limits.x / 2)
            gotoPosition.x = originalPosition.x;
    }
    float GetRandomBetween(int a, int b)
    {
        return (float)Random.Range(a, b) / 10;
    }
    public void ResetPosition()
    {
        transform.position = originalPosition;
    }
}
