using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeper : Character
{
    public Vector2 area_limits;

    public override void MoveTo(int _x, int _y)
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
}
