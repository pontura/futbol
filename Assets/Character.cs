using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [HideInInspector]
    public int teamID;
    public int id;
    public float speed = 5;

    [HideInInspector]  public CharacterActions actions;
    [HideInInspector]  public CharacterSignal characterSignal;

    private void Awake()
    {
        actions = GetComponent<CharacterActions>();
    }
    public void SetPosition(int _x, int _y)
    {
        MoveTo(_x, _y);            
    }
    public void Kick()
    {
        actions.Kick();
    }
    void MoveTo(int _x, int _y)
    {
        if (_x == 0 && _y == 0)
            actions.Idle();
        else
        {
            if(_x != 0)
                actions.LookTo(_x);
            actions.Run();
        }

        transform.Translate(Vector3.right * _x * speed*Time.deltaTime + Vector3.forward * _y * speed * Time.deltaTime);
    }
    public void SetSignal(CharacterSignal signal)
    {
        characterSignal = signal;
        signal.transform.SetParent(actions.transform);
        signal.transform.localScale = Vector3.one;
        signal.transform.localPosition = Vector3.zero;        
    }
}
