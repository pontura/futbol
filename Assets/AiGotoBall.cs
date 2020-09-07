using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiGotoBall : MonoBehaviour
{
    AI ai;

    private void Start()
    {
        ai = GetComponent<AI>();
        Reset();
    }
    public void Init()
    {
        Invoke("ResetGotoBall", Random.Range(0.55f, 1f));
        enabled = true;
    }
    void ResetGotoBall()
    {
        ai.aiPosition.enabled = true;
        enabled = false;
    }
    public void Reset()
    {
        enabled = false;
    }
    private void Update()
    {
        Vector3 pos = transform.position;
        pos.z = Mathf.Lerp(pos.z, ai.ball.transform.position.z, 0.15f);
        transform.position = pos; 
    }
}
