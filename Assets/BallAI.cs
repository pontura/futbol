using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAI : MonoBehaviour
{
    states state;
    enum states
    {
        IDLE,
        PASE
    }
    Character characterReceiving;

    public void Pase(Character characterReceiving)
    {
        CancelInvoke();
        Loop();
        Invoke("Reset", 0.6f);
        this.characterReceiving = characterReceiving;
        state = states.PASE;
    }
    public void Reset()
    {
        CancelInvoke();
        state = states.IDLE;
    }
    void Loop()
    {
        if (state == states.PASE)
        {
            transform.LookAt(characterReceiving.transform.position);
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            //print(characterReceiving.data.avatarName + " " + Time.time);
        }
        Invoke("Loop", 0.1f);
    }
}
