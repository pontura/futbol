using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInGame : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float offset_lookAt;
    float speed = 0.035f;

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
        transform.position = Vector3.Lerp(transform.position, pos, speed);
        transform.localEulerAngles = new Vector3(20, target.position.x * 15 / 20, target.position.x * 5 / 20);
    }
    public void OnGoal(Character character)
    {
        StartCoroutine(GoalCoroutine(character));
        
    }
    IEnumerator GoalCoroutine(Character character)
    {
        yield return new WaitForSeconds(1);
        SetTargetTo(character.transform);
    }
}
