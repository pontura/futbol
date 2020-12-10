using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCatcher : MonoBehaviour
{
    public Transform container;
    Ball ball;
    public states state;
    Character character;

    public enum states
    {
        IDLE,
        GOT_IT,
        WAITING
    }
    private void Start()
    {
        character = GetComponent<Character>();
        container.gameObject.SetActive(false);
    }
    public void Catch(Ball _ball)
    {
        container.gameObject.SetActive(true);
        ball = _ball;
        ball.transform.SetParent(container);
        ball.transform.localPosition = Vector3.zero;
        ball.rb.velocity = Vector3.zero;
        ball.transform.localEulerAngles = new Vector3(0, 0, 0);
        state = states.GOT_IT;
    }
    public void LoseBall()
    {
        character.SetCollidersOff();
        container.gameObject.SetActive(false);
        state = states.WAITING;
        ball = null;
        Invoke("Reset", 0.2f);
    }
    private void Reset()
    {
        state = states.IDLE;
    }
    public void LookAt(Vector3 targetPos)
    {
        targetPos.y = container.transform.position.y;
        container.transform.LookAt(targetPos);
    }
    public void RotateTo(Vector3 targetPos)
    {
        if (targetPos == Vector3.zero) return;
        container.transform.forward = targetPos;
    }
}
