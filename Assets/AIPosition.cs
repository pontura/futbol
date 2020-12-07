using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPosition : MonoBehaviour
{
    public Vector3 originalPosition;
    public Vector3 gotoPosition;
    [HideInInspector] public AI ai;
    public states state;
    public bool isHelper; // lo sigue al qeu tiene la pelota

    public enum states
    {
        GOING,
        WAITING
    }
    private void Awake()
    {
        ai = GetComponent<AI>();
    }
    void Start()
    {        
        originalPosition = transform.position;
        //enabled = false;
    }
    public void Reset()
    {
        CancelInvoke();
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
        CancelInvoke();
        this.enabled = true;
        state = states.GOING;
        if (ai.state == AI.states.DEFENDING)
            gotoPosition = originalPosition;
        else
            SetAttackPosition();
    }
    void SetAttackPosition()
    {       
        CheckHelper();
        if (isHelper)
            UpdateAttackPositionHelper();
        else
            UpdateAttackPosition();        
    }
    void UpdateAttackPositionHelper()
    {
        Invoke("UpdateAttackPositionHelper", 1);
        if (ai.character.type == Character.types.DEFENSOR_DOWN || ai.character.type == Character.types.DEFENSOR_UP)
            ai.character.SuperRun();

        Vector3 characterWithBallPos = ai.characterWithBall.transform.position;
        float offset = GetOffsetToHelper();
        if (ai.character.teamID == 1)
            offset *= -1;

        gotoPosition.x = characterWithBallPos.x - offset;

        gotoPosition.z = originalPosition.z + ((float)Random.Range(-20, 20) / 10);
        SetLimits();
    }
    void UpdateAttackPosition()
    {
        Invoke("UpdateAttackPosition", 1);
        if (ai.character.type == Character.types.DELANTERO_UP || ai.character.type == Character.types.DELANTERO_UP)
            ai.character.SuperRun();
        gotoPosition = originalPosition;
        float goto_x = Mathf.Abs(originalPosition.x) - (Data.Instance.settings.gameplay.limits.x / 2) + ((float)Random.Range(0, 30) / 10);
        if (ai.character.teamID == 2)
            goto_x *= -1;
        gotoPosition.x = Mathf.Lerp(goto_x, ai.ball.transform.position.x, 0.5f);
        gotoPosition.z += ((float)Random.Range(-30, 30) / 10);
        SetLimits();
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
        if (Mathf.Abs(gotoPosition.z) > Data.Instance.settings.gameplay.limits.y / 2)
            gotoPosition.z = originalPosition.z;
        if (Mathf.Abs(gotoPosition.x) > Data.Instance.settings.gameplay.limits.x / 2)
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
    void CheckHelper()
    {
        isHelper = false;
        if (ai.characterWithBall == null)
            return;

        if (ai.characterWithBall.teamID == ai.character.teamID)
        {
            switch(ai.characterWithBall.type)
            {
                case Character.types.CENTRAL: break;
                case Character.types.GOALKEEPER:
                    if (ai.character.type == Character.types.CENTRAL) isHelper = true; break;
                case Character.types.DEFENSOR_DOWN:
                    if(ai.character.type == Character.types.DEFENSOR_UP) isHelper = true; break;
                case Character.types.DEFENSOR_UP:
                    if (ai.character.type == Character.types.DEFENSOR_DOWN) isHelper = true; break;
                case Character.types.DELANTERO_DOWN:
                    if (ai.character.type == Character.types.DELANTERO_UP) isHelper = true; break;
                case Character.types.DELANTERO_UP:
                    if (ai.character.type == Character.types.DELANTERO_DOWN) isHelper = true; break;
            }
        }
    }
    float GetOffsetToHelper()
    {
        if (ai.characterWithBall == null)
            return 0;
        switch (ai.characterWithBall.type)
        {
            case Character.types.CENTRAL: return 0;
            case Character.types.GOALKEEPER:
                return Random.Range(3, 6);
            case Character.types.DEFENSOR_DOWN:
                return Random.Range(0, 2);
            case Character.types.DEFENSOR_UP:
                return Random.Range(0, 2);
            case Character.types.DELANTERO_DOWN:
                return Random.Range(-1, 1);
            default:
                return Random.Range(-1, 1);
        }
    }
}
