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
    public void SetTargetTo(Transform t)
    {
        target = t;
    }
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = target.position.x * 15 / 20;
        transform.position = Vector3.Lerp(transform.position, pos, 0.1f);
        transform.localEulerAngles = new Vector3(20, target.position.x * 15 / 20, target.position.x * 5 / 20);
        //Vector3 ballPos = Vector3.Lerp(transform.position, target.transform.position, 0.1f);
        //ballPos.x /= offset_lookAt;
        //ballPos.y = offset.y;
        //ballPos.z = offset.z;
        //transform.LookAt(ballPos);
    }
    public void OnGoal(Character character)
    {
        SetTargetTo(character.transform);
    }
}
