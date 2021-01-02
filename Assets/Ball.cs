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
        Events.OnRestartGame += OnRestartGame;
        limits = new Vector2(Data.Instance.stadiumData.active.size_x, Data.Instance.stadiumData.active.size_y);
        container = transform.parent;
        this.rb = GetComponent<Rigidbody>();
        Reset();
    }
    private void OnDestroy()
    {
        Events.OnRestartGame -= OnRestartGame;
    }
    void OnRestartGame()
    {
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

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(GetForwardPosition(4), 0.5f);
        Gizmos.color = Color.red;
    }
#endif

    public Vector3 GetForwardPosition(float value)
    {
        return transform.position + transform.forward * value;
    }
    public void SetPlayer(Character _character)
    {
        this.character = _character;
    }
    public Character GetCharacter()
    {
        return character;
    }
    void CharacterCatchBall(Character character)
    {
        timeCatched = Time.time;
        characterThatKicked = character;
        character.OnCatch(this);

        if (this.character != null)
            this.character.ballCatcher.LoseBall();

        this.character = character;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (Game.Instance.state != Game.states.PLAYING)
            return;
        if (other.gameObject.tag == "Goal")
        {
            int teamID = 1;
            if (transform.position.x > 0) teamID = 2;
            Game.Instance.Goal(teamID, characterThatKicked);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {      
        if (Game.Instance.state != Game.states.PLAYING)
            return;
        if (character != null && character.type == Character.types.GOALKEEPER)
            return;
        if(collision.gameObject.tag == "GoalPalo")
        {
            Events.PlaySound("shouts", "palo", false);
            if (collision.gameObject.transform.position.y > 2.5f)
                VoicesManager.Instance.SayPalo(0);
            else
                VoicesManager.Instance.SayPalo(1);
        }
        else if (collision.gameObject.tag == "lateral")
        {
            Events.PlaySound("dogs", "wallBack_" + Random.Range(1, 6), false);
        } else if (collision.gameObject.tag == "Corner")
        {
            Events.PlaySound("dogs", "wallFront_" + Random.Range(1, 3), false);
        }
        else if (collision.gameObject.tag == "Referi")
        {
            Character character = collision.gameObject.GetComponent<Character>();
            character.actions.Kick(CharacterActions.kickTypes.HEAD);
        }
         else if (collision.gameObject.tag == "Player")
        {
            Character character = collision.gameObject.GetComponent<Character>();

            if (transform.localPosition.y < 0.9f && (this.character == null || (character != this.character)) )//character.ballCatcher.state == BallCatcher.states.IDLE)
                CharacterCatchBall(character);
            else if (character.type == Character.types.GOALKEEPER)
            {
                int rand = Random.Range(0, 100);
                if(rand < Data.Instance.settings.gameplay.gk_CatchOnAir)
                    CharacterCatchBall(character);
                else
                    character.actions.GoalKeeperJump();
            }                
            else if (transform.localPosition.y > 1.5f &&
                      (character.teamID == 1 && transform.position.x < -limits.x / 5
                    || character.teamID == 2 && transform.position.x > limits.x / 5))
            {
                characterThatKicked = character;
                character.SetCollidersOff();

                if(character.actions.state != CharacterActions.states.JUMP)
                    character.actions.Kick(CharacterActions.kickTypes.CHILENA);

                AimGoal(character);
                Kick(CharacterActions.kickTypes.CHILENA);
            }
            else
            {
                characterThatKicked = character;
                character.SetCollidersOff();
                transform.eulerAngles = character.ballCatcher.container.transform.eulerAngles;

                if (character.actions.state != CharacterActions.states.JUMP)
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
    public void Kick(CharacterActions.kickTypes kickType, float forceForce = 0)
    {
        float force = 1;
        if (kickType == CharacterActions.kickTypes.HARD && uIForce.GetForce() > 0.6f)
            kickType = CharacterActions.kickTypes.KICK_TO_GOAL;

       
        if (kickType == CharacterActions.kickTypes.KICK_TO_GOAL)
        {
            character.SetCollidersOff();
            AimGoal(character, 40);
        }
        else if (forceForce != 0)
            force = forceForce;
        else if (
        kickType != CharacterActions.kickTypes.HEAD 
        || kickType != CharacterActions.kickTypes.CHILENA
        )
            force = AddForceToKick();

        FreeBall();

        //El arquero saca por abajo:
        if (character != null && (character.type == Character.types.GOALKEEPER && kickType == CharacterActions.kickTypes.SOFT))
            transform.localPosition = new Vector3(transform.localPosition.x, -0.17f, transform.localPosition.z);

        
        Vector3 dir = transform.forward;
       
        switch (kickType)
        {
            case CharacterActions.kickTypes.HARD:
                Events.PlaySound("common", "kick3", false);
                dir *= Data.Instance.settings.gameplay.kickHard* force;
                dir += Vector3.up * Data.Instance.settings.gameplay.kickHardAngle * force;
                break;
            case CharacterActions.kickTypes.SOFT:
                Events.PlaySound("common", "kick2", false);
                dir *= Data.Instance.settings.gameplay.kickSoft * force;
                dir += Vector3.up * Data.Instance.settings.gameplay.kickSoftAngle * force;
                break;
            case CharacterActions.kickTypes.BALOON:
                Events.PlaySound("common", "kick1", false);
                dir *= Data.Instance.settings.gameplay.kickBaloon * force;
                dir += Vector3.up * Data.Instance.settings.gameplay.kickBaloonAngle * force;
                break;
            case CharacterActions.kickTypes.HEAD:
                Events.PlaySound("common", "kick1", false);
                dir *= Data.Instance.settings.gameplay.kickHead * force;
                dir += Vector3.up * Data.Instance.settings.gameplay.kickHeadAngle * force;
                break;
            case CharacterActions.kickTypes.CHILENA:
                Events.PlaySound("common", "kick3", false);
                dir *= Data.Instance.settings.gameplay.kickChilena * force;
                dir += Vector3.up * Data.Instance.settings.gameplay.kickChilenaAngle * force;
                break;
            case CharacterActions.kickTypes.KICK_TO_GOAL:
                Events.PlaySound("common", "kick3", false);
                dir *= Data.Instance.settings.gameplay.kickHard * 1.5f;
                dir += Vector3.up * Data.Instance.settings.gameplay.kickHardAngle * force;
                break;
            case CharacterActions.kickTypes.CENTRO:
                Events.PlaySound("common", "kick3", false);
                dir *= Data.Instance.settings.gameplay.kickCentro * 1.5f;
                dir += Vector3.up * Data.Instance.settings.gameplay.kickCentroAngle * force;
                break;
        }
        rb.velocity = Vector3.zero;
        rb.AddForce(dir);
        Events.OnBallKicked(kickType, forceForce, character);
        character = null;
    }
    public void OnSetApplyForce(Vector3 dir, Character character)
    {
        rb.AddForce(dir);
        Events.OnBallKicked(CharacterActions.kickTypes.DESPEJE_GOALKEEPER, 0, character);
    }
    public float GetDurationOfBeingCatch()
    {
        return Time.time - timeCatched;
    }
}
