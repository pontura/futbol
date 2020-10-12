﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInGame : MonoBehaviour
{
    Animation anim;
    public Camera cam;
    float initial_y_position;
    float speed_to_tribuna = 20;
    public float camera_tribuna_y;
    Transform target;
    public Vector3 offset;
    public float offset_lookAt;
    float speed = 0.035f;
    bool filmingPlayer;
    float filming_y;

    public float originalSize = 5;
    public float zoomSize = 2;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        cam = GetComponent<Camera>();
    }
    private void Start()
    {
        initial_y_position = transform.position.y;
    }
    public void Restart()
    {
        if (anim != null)
        {
            anim.enabled = true;
            anim.Stop();           
            anim.Play();
        }        
    }
    public void Reset()
    {
        if (anim != null)
            anim.enabled = false;
        ResetShootingPlayer();
        cam.orthographicSize = originalSize;
    }
    public void SetTargetTo(Transform t)
    {        
        target = t;
    }
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = target.position.x * 0.8f;
        if (filmingPlayer)
            pos.y = filming_y;
        else if (Game.Instance.state == Game.states.PLAYING || Game.Instance.state == Game.states.WAITING)
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
        yield return new WaitForSeconds(2);        
        ShootingPlayer(character);
    }
    public void ShootPlayer(Character character, float duration = 0)
    {
        ShootingPlayer(character);
        if(duration > 0)
            Invoke("ResetShootingPlayer", duration);
    }
    void ResetShootingPlayer()
    {
        filmingPlayer = false;
    }
    void ShootingPlayer(Character character)
    {
        cam.orthographicSize = zoomSize;
        filmingPlayer = true;
        filming_y = initial_y_position + character.transform.position.z / 2.5f;
    }
}
