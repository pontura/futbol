using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Transform container;
    public Rigidbody rb;
    Character character;

    void Start()
    {
        container = transform.parent;
        this.rb = GetComponent<Rigidbody>();
    }
    public void SetPlayer(Character _character)
    {
        this.character = _character;
    }
    public Character GetCharacter()
    {
        return character;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Character lastCharacterWithBall = null;
        if (character != null)
            lastCharacterWithBall = character;

        if (collision.gameObject.tag == "Player")
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (transform.localPosition.y < 0.37f && character.ballCathcer.state == BallCatcher.states.IDLE)
            {
                if (lastCharacterWithBall != null)
                    lastCharacterWithBall.ballCathcer.LoseBall();

                character.OnCatch(this);
                this.character = character;
                rb.constraints = RigidbodyConstraints.FreezeAll;

                
            }
        }
    }
    void FreeBall()
    {
        rb.constraints = RigidbodyConstraints.None;
        transform.SetParent(container);
    }
    public void Kick(float force)
    {
        FreeBall();
        character = null;
        Vector3 dir = transform.forward * force;
        dir += Vector3.up * force / 4;
        rb.AddForce(dir);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Character character = other.GetComponent<Character>();
            character.OnBallTriggerEnter(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Character character = other.GetComponent<Character>();
            character.OnBallTriggerExit(this);
        }
    }
}
