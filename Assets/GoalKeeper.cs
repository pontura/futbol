using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeper : Character
{
    public Vector2 area_limits;

    public override void MoveTo(float _x, float _y)
    {
        if (teamID == 1)
        {
            if (_x < 0 && transform.position.x < area_limits.x) _x = 0;
        }
        else
        {
            if (_x > 0 && transform.position.x > -area_limits.x) _x = 0;
        }

        if (_y > 0 && transform.position.z > area_limits.y || _y < 0 && transform.position.z < -area_limits.y) _y = 0;

        if (_x == 0 && _y == 0)
            actions.Idle();
        else
        {
            actions.Run();
        }

        transform.Translate(Vector3.right * _x * speed * Time.deltaTime + Vector3.forward * _y * speed * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (Game.Instance.state != Game.states.PLAYING) return;
        if (collision.gameObject.tag == "Ball")
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();
            if (ball.character == this)
                return;
            actions.GoalKeeperHands();
            ball.OnSetApplyForce(new Vector3(Random.Range(100, 300), Random.Range(400,900),0), this);
        }
    }
}
