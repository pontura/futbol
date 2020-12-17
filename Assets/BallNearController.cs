using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallNearController : MonoBehaviour
{
    AI ai;

    void Start()
    {
        ai = GetComponent<AI>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (ai.character.isBeingControlled) return;
        if(other.tag == "Ball")
        {
            Ball ball = other.GetComponent<Ball>();

            if (ball.transform.position.y > 2.25f)
                ai.OnBallNearOnAir();
            else
                ai.OnBallNear();
        }
    }
}
