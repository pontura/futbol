using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCatcher : MonoBehaviour
{
    public Transform container;
    Ball ball;
    states state;
    Character character;
    public SpriteRenderer arrow;
    public SpriteRenderer colaArrow;
    Animation anim;
    float arrow_pos_z_initial;
    float arrow_pos_max_z = 17;
    float speed = 10;
    public GameObject cola;

    enum states
    {
        IDLE,
        GOT_IT,
        WAITING,
        JUEGUITO,
        KICKING
    }
    private void Start()
    {
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
        ball.transform.SetParent(container);
        ball.transform.localPosition = Vector3.zero;
        ball.rb.velocity = Vector3.zero;
        ball.transform.localEulerAngles = new Vector3(0, 0, 0);
        state = states.GOT_IT;
        if (character.lineSignal != null)
            character.lineSignal.SetOn(true);
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
    public void Reset()
    {
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
    public void InitKick(int buttonID)
    {
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
        if (state != states.KICKING) return;

        Vector3 pos = arrow.transform.localPosition;
        pos.z += speed * Time.deltaTime;
        if (pos.z > arrow_pos_max_z) pos.z = arrow_pos_max_z;
        arrow.transform.localPosition = pos;
        cola.transform.localScale = new Vector3(1, 1, (1+(pos.z-arrow_pos_z_initial)*0.7f));
    }
}
