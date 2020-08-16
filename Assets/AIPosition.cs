using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPosition : MonoBehaviour
{
    Vector3 originalPosition;
    Vector3 gotoPosition;
    public AI ai;
    System.Action OnDone;

    void Start()
    {
        ai = GetComponent<AI>();
        originalPosition = transform.position;
        enabled = false;
    }
    void Update()
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
        } else
        {
            ai.character.actions.Idle();
            this.enabled = false;
            OnDone();
        }
    }
    public virtual void SetActive(System.Action OnDone)
    {
        this.enabled = true;
        this.OnDone = OnDone;
        if (ai.state == AI.states.DEFENDING)
            gotoPosition = originalPosition;
        else
        {
            gotoPosition = originalPosition;
            if(ai.character.teamID == 1)
                gotoPosition.x = originalPosition.x - Data.Instance.settings.limits.x / 2 + ((float)Random.Range(0,30)/10);
            else
                gotoPosition.x = originalPosition.x + Data.Instance.settings.limits.x / 2 - ((float)Random.Range(0, 30)/10);

            gotoPosition.z += ((float)Random.Range(-30, 30) / 10);
        }
    }
}
