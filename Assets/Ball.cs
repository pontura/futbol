using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Transform container;
    public Rigidbody rb;
    Character character;
    public Character characterThatKicked;
    Vector3 limits;

    void Start()
    {
        limits = Data.Instance.settings.limits;
        container = transform.parent;
        this.rb = GetComponent<Rigidbody>();
        Reset();
    }
    public void KickIfOnGoal()
    {
        if (character != null)
            Kick(CharacterActions.kickTypes.HARD);
    }
    public void Reset()
    {
     
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(0, 3, 0);
        FreeBall();
    }
    private void Update()
    {
        if (Game.Instance.state != Game.states.PLAYING)
            return;

        Vector3 velocity = rb.velocity;
        if(transform.position.y<2.6f && transform.position.z< 4.5f && transform.position.z > -4.5f)
        {
            if (transform.position.x <= -limits.x / 2)
                Game.Instance.Goal(1, characterThatKicked);
            else if (transform.position.x >= limits.x / 2)
                Game.Instance.Goal(2, characterThatKicked);
        } else
        if (transform.position.x >= limits.x/2 && rb.velocity.x > 0 || transform.position.x <= -limits.x / 2 && rb.velocity.x < 0)
            velocity.x *= -1;
        else if (transform.position.z >= limits.y / 2 && rb.velocity.z > 0 || transform.position.z <= -limits.y / 2 && rb.velocity.z < 0)
            velocity.z *= -1;
        rb.velocity = velocity;
        Vector3 pos = transform.position;
        if (transform.position.y < 0)
            pos.y = 1;
        transform.position = pos;
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
      
        if (Game.Instance.state != Game.states.PLAYING)
            return;

        Character lastCharacterWithBall = null;
        if (character != null)
            lastCharacterWithBall = character;

        if (collision.gameObject.tag == "Player")
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (transform.localPosition.y < 1f && character.ballCathcer.state == BallCatcher.states.IDLE)
            {
                if (lastCharacterWithBall != null)
                {
                    if (lastCharacterWithBall.isGoldKeeper)
                        Game.Instance.charactersManager.GoalKeeperLoseBall(lastCharacterWithBall.id);

                    lastCharacterWithBall.ballCathcer.LoseBall();
                }
                characterThatKicked = character;
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
    public void Kick(CharacterActions.kickTypes kickType)
    {
        FreeBall();
        character = null;
        Vector3 dir = transform.forward;
        switch (kickType)
        {
            case CharacterActions.kickTypes.HARD:
                dir *= Data.Instance.settings.kickHard;
                dir += Vector3.up * Data.Instance.settings.kickHardAngle;
                break;
            case CharacterActions.kickTypes.SOFT:
                dir *= Data.Instance.settings.kickSoft;
                dir += Vector3.up * Data.Instance.settings.kickSoftAngle;
                break;
            case CharacterActions.kickTypes.BALOON:
                dir *= Data.Instance.settings.kickBaloon;
                dir += Vector3.up * Data.Instance.settings.kickBaloonAngle;
                break;
        }
       
        rb.AddForce(dir);
        Events.OnBallKicked();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Game.Instance.state != Game.states.PLAYING)
            return;
        if (other.tag == "Player")
        {
            Character character = other.GetComponent<Character>();
            character.OnBallTriggerEnter(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (Game.Instance.state != Game.states.PLAYING)
            return;
        if (other.tag == "Player")
        {
            Character character = other.GetComponent<Character>();
            character.OnBallTriggerExit(this);
        }
    }
}
