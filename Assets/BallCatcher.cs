using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCatcher : MonoBehaviour
{
    public Transform container;
    Ball ball;
    public states state;
    public enum states
    {
        IDLE,
        GOT_IT,
        WAITING
    }
    private void Start()
    {
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
    }
    public void LoseBall()
    {
        container.gameObject.SetActive(false);
        state = states.WAITING;
        ball = null;
        Invoke("Reset", 1);
    }
    private void Reset()
    {
        state = states.IDLE;
    }
}
