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
        Invoke("Reset", 0.25f);
    }
    private void Reset()
    {
        state = states.IDLE;
    }
    public void SetRotation(int _x, int _y)
    {
        if (_x == 0 && _y == 0)
            return;
        float rot = 0;
        if (_x == 1 && _y == 0)
            rot = 90;
        else if (_x == 1 && _y == -1)
            rot = 90 + 45;
        else if (_x == 0 && _y == -1)
            rot = 180;
        else if (_x == -1 && _y == -1)
            rot = 180 + 45;
        else if (_x == -1 && _y == 0)
            rot = 270;
        else if (_x == -1 && _y == 1)
            rot = 270 + 45;
        Vector3 newRot = new Vector3(0, rot, 0);
        container.eulerAngles = Vector3.Lerp(container.transform.eulerAngles, newRot, 0.1f);
    }
}
