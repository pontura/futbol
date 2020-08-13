using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    void Update()
    {
        Vector3 pos = target.position;
        pos.y = transform.position.y;
        transform.position = pos;
    }
}
