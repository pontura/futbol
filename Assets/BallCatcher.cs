﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCatcher : MonoBehaviour
{
    public Transform container;
    public GameObject signal;
    Ball ball;
    states state;
    Character character;
    public SpriteRenderer arrow;
    public SpriteRenderer colaArrow;
    Animation anim;
    float arrow_pos_z_initial;
    float arrow_pos_max_z = 14;
    float speed;
    float value;
    public GameObject cola;
    AimState aimState;
    public enum AimState
    {
        STANDARD,
        CENTRO
    }

    enum states
    {
        IDLE,
        GOT_IT,
        WAITING,
        JUEGUITO,
        RUN,
        RUN_FAST,
        KICKING
    }
    private void Start()
    {
        speed = Data.Instance.settings.forceBarSpeed * 8;
        container.gameObject.SetActive(false);
        anim = container.GetComponent<Animation>();
        character = GetComponent<Character>();
        Show(false);
        arrow_pos_z_initial = arrow.transform.localPosition.z;
    }
    
    public void Catch(Ball _ball)
    {
        Reset();
        container.gameObject.SetActive(true);
        ball = _ball;
        ball.Catched(container);
        state = states.GOT_IT;

        if (ball.character == null || ball.character != character)
            signal.SetActive(false);
        else
            signal.SetActive(true);

        if (character.lineSignal != null)
        {
            signal.SetActive(true);
            character.lineSignal.SetOn(true);
        }
    }
    public void LoseBall()
    {
        character.SetCollidersOff(Data.Instance.settings.gameplay.freeze_by_loseBall);
        state = states.WAITING;
        ball = null;
        Invoke("Reset", 0.2f);
        container.gameObject.SetActive(false);
        if (character.lineSignal != null)
            character.lineSignal.SetOn(false);
    }
    void Reset()
    {
        CancelInvoke();
        container.gameObject.SetActive(false);
        cola.transform.localScale = Vector3.one;
        Vector3 pos = arrow.transform.localPosition;
        pos.z = arrow_pos_z_initial;
        arrow.transform.localPosition = pos;
        colaArrow.color = Color.white;
        arrow.color = Color.white;
        state = states.IDLE;
        if(character.type != Character.types.GOALKEEPER)
            anim.Play("ball_idle");
    }
    public void LookAt(Vector3 targetPos)
    {
        targetPos.y = container.transform.position.y;
        container.transform.LookAt(targetPos);
        ball.ForcePosition(container);
    }
    public void RotateTo(Vector3 targetPos)
    {
        if (targetPos == Vector3.zero) return;
        container.transform.forward = targetPos;
    }
    public void Show(bool isOn)
    {
        arrow.enabled = isOn;
    }
    public void Jump()
    {
        anim.Play("ball_Jump");
    }
    public void Jueguito()
    {
        state = states.JUEGUITO;
        anim.Play("ball_jueguito");
    }
    public void ResetFastRun()
    {
        state = states.RUN;
    }
    public void Run(bool fast)
    {
        if (state == states.KICKING)  return;
        if (fast)
        {
            state = states.RUN_FAST;
            anim.Play("ball_run_fast");
        }
        else if (state != states.RUN_FAST)
        {
            state = states.RUN;
            anim.Play("ball_run");
        }
    }
    public void Idle()
    {
        state = states.GOT_IT;
        anim.Play("ball_idle");
    }
    public void ResetJueguito()
    {
        Idle();
    }
    public void InitKick(int buttonID)
    {
        value = 0;
        Color arrowColor = Color.yellow;
        switch(buttonID)
        {
            case 1:
                arrowColor = Color.red;
                break;
        }
        arrow.color = arrowColor;
        colaArrow.color = arrowColor;
        state = states.KICKING;
    }
    private void Update()
    {
        if (state == states.KICKING)
        {
            Vector3 pos = arrow.transform.localPosition;
            value += speed * Time.deltaTime;
            pos.z = arrow_pos_z_initial + value;
            if (pos.z > arrow_pos_max_z) pos.z = arrow_pos_max_z;
            arrow.transform.localPosition = pos;
            cola.transform.localScale = new Vector3(1, 1, (1 + (pos.z - arrow_pos_z_initial) * 0.7f));
        }
    }
    public float GetForce()
    {
        float v = (value) / (arrow_pos_max_z - arrow_pos_z_initial);
        return v;
    }
}
