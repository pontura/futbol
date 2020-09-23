using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInGame : MonoBehaviour
{
    public Camera cam;
    float initial_y_position;
    float speed_to_tribuna = 20;
    public float camera_tribuna_y;
    public Transform target;
    public Vector3 offset;
    public float offset_lookAt;
    float speed = 0.035f;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    private void Start()
    {        
        initial_y_position = transform.position.y;
    }
    public void SetTargetTo(Transform t)
    {
        target = t;
    }
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = target.position.x * 0.8f;
        if (Game.Instance.state == Game.states.PLAYING)
            pos.y = initial_y_position;
        else if (pos.y < camera_tribuna_y)
            pos.y += Time.deltaTime * speed_to_tribuna;
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
