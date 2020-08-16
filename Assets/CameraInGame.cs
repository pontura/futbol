using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInGame : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float offset_lookAt;
    void Start()
    {
        
    }
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = target.position.x / 1.2f;
        transform.position = Vector3.Lerp(transform.position, pos, 0.2f);

        Vector3 ballPos = Vector3.Lerp(transform.position, target.transform.position, 0.1f);
        ballPos.x /= offset_lookAt;
        ballPos.y = offset.y;
        ballPos.z = offset.z;
        transform.LookAt(ballPos);
    }
}
