using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public UIForce uIForce;
    Transform container;
    public Rigidbody rb;
    public Character character;
    public Character characterThatKicked;
    Vector3 limits;
    float timeCatched;


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
        if(transform.position.y<2.9f && transform.position.z< 4.5f && transform.position.z > -4.5f)
        {
            if (transform.position.x <= (-limits.x / 2) + 0.25f)
                Game.Instance.Goal(1, characterThatKicked);
            else if (transform.position.x >= (limits.x / 2) - 0.25f)
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
        if (character != null && character.isGoldKeeper)
            return;

        if (collision.gameObject.tag == "Goal")
        {
            rb.velocity = Vector3.zero;
        }
        else if (collision.gameObject.tag == "Referi")
        {
            Character character = collision.gameObject.GetComponent<Character>();
            character.actions.Kick(CharacterActions.kickTypes.HEAD);
        }
         else   if (collision.gameObject.tag == "Player")
        {
            Character character = collision.gameObject.GetComponent<Character>();

            if (transform.localPosition.y < 0.9f && character != this.character)//character.ballCatcher.state == BallCatcher.states.IDLE)
            {
                timeCatched = Time.time;
                characterThatKicked = character;
                character.OnCatch(this);
                if(this.character != null)
                    this.character.ballCatcher.LoseBall();
                this.character = character;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }
            else if(character.isGoldKeeper)
            {
                character.actions.GoalKeeperJump();
            }
            else if (transform.localPosition.y >1.5f &&
                      (character.teamID == 1 && transform.position.x < -Data.Instance.settings.limits.x / 5 
                    || character.teamID == 2 && transform.position.x >  Data.Instance.settings.limits.x / 5))
            {
                character.SetCollidersOff();
                character.actions.Kick(CharacterActions.kickTypes.CHILENA);
                AimGoal(character);
                Kick(CharacterActions.kickTypes.CHILENA);               
            }
            else {
                character.SetCollidersOff();
                transform.eulerAngles = character.ballCatcher.container.transform.eulerAngles;
                character.actions.Kick(CharacterActions.kickTypes.HEAD);
                Kick(CharacterActions.kickTypes.HEAD);
            }
        }
    }
    public void AimGoal(Character character, float randomYRotation = 0)
    {
        Vector3 lookTo = Vector3.zero;
        if (character.teamID == 1)
            lookTo.y = -90 - (character.transform.position.z * 5);
        else
            lookTo.y = 90 + (character.transform.position.z * 5);

        if (randomYRotation > 0)
            lookTo.y += Random.Range(-randomYRotation, randomYRotation);
        transform.eulerAngles = lookTo;
    }
    void FreeBall()
    {
        rb.constraints = RigidbodyConstraints.None;
        transform.SetParent(container);
    }
    float AddForceToKick()
    {       
        float force = uIForce.GetForce();
        if (force <= 0 && character != null)
        {
            if (character.transform.localScale.x == -1)
                character.MoveTo(1, 0);
            else
                character.MoveTo(-1, 0);
            force = 0.4f;
        }
        else
            force += 1;
        return force;
    }
    public void Kick(CharacterActions.kickTypes kickType)
    {
        float force = 1;
        if (kickType == CharacterActions.kickTypes.HARD && uIForce.GetForce() > 0.6f)
            kickType = CharacterActions.kickTypes.KICK_TO_GOAL;
        if (kickType == CharacterActions.kickTypes.KICK_TO_GOAL)
        {
            character.SetCollidersOff();
            AimGoal(character, 40);
        } else if (   
            kickType != CharacterActions.kickTypes.HEAD 
            || kickType != CharacterActions.kickTypes.CHILENA
            )
            force = AddForceToKick();

        FreeBall();
        character = null;
        Vector3 dir = transform.forward;
       
        switch (kickType)
        {
            case CharacterActions.kickTypes.HARD:
                Events.PlaySound("common", "kick3", false);
                dir *= Data.Instance.settings.kickHard* force;
                dir += Vector3.up * Data.Instance.settings.kickHardAngle * force;
                break;
            case CharacterActions.kickTypes.SOFT:
                Events.PlaySound("common", "kick2", false);
                dir *= Data.Instance.settings.kickSoft * force;
                dir += Vector3.up * Data.Instance.settings.kickSoftAngle * force;
                break;
            case CharacterActions.kickTypes.BALOON:
                Events.PlaySound("common", "kick1", false);
                dir *= Data.Instance.settings.kickBaloon * force;
                dir += Vector3.up * Data.Instance.settings.kickBaloonAngle * force;
                break;
            case CharacterActions.kickTypes.HEAD:
                Events.PlaySound("common", "kick1", false);
                dir *= Data.Instance.settings.kickHead * force;
                dir += Vector3.up * Data.Instance.settings.kickHeadAngle * force;
                break;
            case CharacterActions.kickTypes.CHILENA:
                Events.PlaySound("common", "kick3", false);
                dir *= Data.Instance.settings.kickChilena * force;
                dir += Vector3.up * Data.Instance.settings.kickChilenaAngle * force;
                break;
            case CharacterActions.kickTypes.KICK_TO_GOAL:
                Events.PlaySound("common", "kick3", false);
                dir *= Data.Instance.settings.kickHard * 1.5f;
                dir += Vector3.up * Data.Instance.settings.kickHardAngle * force;
                break;
        }
        rb.velocity = Vector3.zero;
        rb.AddForce(dir);
        Events.OnBallKicked();
    }
    public float GetDurationOfBeingCatch()
    {
        return Time.time - timeCatched;
    }
}
