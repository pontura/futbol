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
        if (ai == null) return;
        if (ai.character == null) return;
        if (ai.character.isBeingControlled) return;
        
        if(other.tag == "Ball")
        {
            Ball ball = other.GetComponent<Ball>();
            if (ai.character == ball.characterThatKicked)
                return;
            if (ball.transform.position.y > 2.25f)
                ai.OnBallNearOnAir();
            else if(ai.character.type == Character.types.GOALKEEPER && ball.transform.position.y > 1f)
                ai.OnBallNearOnAir();            
            else
                ai.OnBallNear();
        }
    }
}
