using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInGame : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Start()
    {
        
    }
    void Update()
    {
        Vector3 pos = transform.localPosition;
        pos.x = target.localPosition.x / 1.2f;
        transform.localPosition = Vector3.Lerp(transform.position, pos, 0.2f);
       // Vector3 ballPos = Vector3.Lerp(transform.position, target.transform.position, 0.1f);
        //ballPos.x /=1.5f;
        //ballPos.y = offset.y;
        //ballPos.z = offset.z;
        //transform.LookAt(ballPos);
    }
}
