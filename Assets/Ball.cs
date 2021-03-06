﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Transform container;
    BallAI ballAI;
    public Rigidbody rb;
    public Character character;
    public Character characterThatKicked;
    Vector3 limits;
    float timeCatched;
    CombaFX combaFX;

    float catchedOffsetY = -1f;
    float offsetForward;
    float smooth;

    void Start()
    {
        offsetForward = Data.Instance.settings.ball_offsetForward;
        smooth = Data.Instance.settings.ball_smooth;
        ballAI = GetComponent<BallAI>();
        Events.OnRestartGame += OnRestartGame;
        limits = new Vector2(Data.Instance.stadiumData.active.size_x, Data.Instance.stadiumData.active.size_y);
        container = transform.parent;
        this.rb = GetComponent<Rigidbody>();
        combaFX = GetComponent<CombaFX>();
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
    Settings.GamePlay GetStats()
    {
        if (character != null) return character.stats; return Data.Instance.settings.gameplay;
    }
    public Vector3 GetForwardPosition(float value)
    {
        Vector3 to = transform.position + (transform.forward * value);
       // print(transform.position + "   _____________ to ______________" + to);
        return to;
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
        if(combaFX != null)
            combaFX.Reset();

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

            ballAI.Reset();
            Character character = collision.gameObject.GetComponent<Character>();

            if (transform.localPosition.y < character.stats.height_to_dominate_ball)
                CheckToGetBall(character);
            else if (character.type == Character.types.GOALKEEPER)
            {
                int rand = Random.Range(0, 100);
                if (rand < character.stats.gk_CatchOnAir)
                    CharacterCatchBall(character);
            }            
            else if (character.CheckIfIsInAreaRival() && transform.localPosition.y > 1.2f)
            {
                characterThatKicked = character;
                character.actions.Kick(CharacterActions.kickTypes.CHILENA);
                transform.LookAt(AimGoal(character));
                Kick(CharacterActions.kickTypes.CHILENA);
            }
            else if (transform.localPosition.y > 1.5f)
            {
                character.actions.Kicked();
            }
            else
            {
                characterThatKicked = character;
                transform.eulerAngles = character.ballCatcher.container.transform.eulerAngles;

                if (character.actions.state != CharacterActions.states.JUMP)
                    character.actions.Kick(CharacterActions.kickTypes.HEAD);

                Kick(CharacterActions.kickTypes.HEAD);
            }
        }
    }
    void CheckToGetBall(Character characterToCatch)
    {
        bool canCatch = false;
        if (character == null) canCatch = true;
        else if (!character.isBeingControlled && characterToCatch.actions.state == CharacterActions.states.DASH && Random.Range(0, 10) < GetStats().random_jump_a_dash)
        {
            character.Dash_with_ball();
            canCatch = false;
            Events.PlaySound("common2", "risa", false);
        }
        else if (characterToCatch.actions.state == CharacterActions.states.FREEZE) canCatch = false;
        else if (character.actions.state == CharacterActions.states.DASH_WITH_BALL)
        {
            Events.PlaySound("common2", "risa", false);
            canCatch = false;
        }
        else if (characterToCatch.actions.state == CharacterActions.states.DASH) canCatch = true;
        else if (character != characterToCatch && (Random.Range(0, 10) < GetStats().random_steal_ball || characterToCatch.isBeingControlled))
        {
            canCatch = true;
        }
        if (canCatch)
            CharacterCatchBall(characterToCatch);
        else
            characterToCatch.SetCollidersOff(0.25f);
    }
    Vector3 AimGoal(Character character)
    {
        float goalX = Data.Instance.stadiumData.active.size_x / 2;
        if (character.teamID == 1) goalX *= -1;
        Vector3 goalPos = new Vector3(goalX, 0, Random.Range(-6f, 6f));
        return goalPos;
    }
    void FreeBall()
    {
        customContainer = null;
        rb.constraints = RigidbodyConstraints.None;
        transform.SetParent(container);
    }
    float AddForceToKick()
    {
        if (character == null || character.ballCatcher == null) return 0;
        float force = character.ballCatcher.GetForce()*1.5f;
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
        ballAI.Reset();
        float force = 1;
        if (kickType == CharacterActions.kickTypes.HARD && character.ballCatcher.GetForce() > 0.5f)
        {
            if(character != null && !character.isBeingControlled)
                kickType = CharacterActions.kickTypes.KICK_TO_GOAL;
        }

        

        if (kickType == CharacterActions.kickTypes.KICK_TO_GOAL && !character.isBeingControlled)
        {
            character.ballCatcher.LookAt( AimGoal(character) );
            force = Random.Range(1.5f, 2.7f);
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
                KickBallSound();
                float kickHard = GetStats().kickHard * 1.25f;
                if (character != null && 
                    (character.actions.state == CharacterActions.states.JUEGUITO)
                    ||
                     (character.actions.state == CharacterActions.states.DASH_WITH_BALL)
                    )
                {
                    kickHard *= 2;
                }
                  
                dir *= kickHard * force;
                dir += Vector3.up * GetStats().kickHardAngle * (force/1.1f);               
                break;
            case CharacterActions.kickTypes.SOFT:
                CharactersManager cm = Game.Instance.charactersManager;
                KickBallSound();
                dir *= GetStats().kickSoft * force;
                dir += Vector3.up * GetStats().kickSoftAngle * force;
                break;
            case CharacterActions.kickTypes.BALOON:
                KickBallSound();
                dir *= GetStats().kickBaloon * force;
                dir += Vector3.up * GetStats().kickBaloonAngle * force;
                if (character != null && character.type == Character.types.GOALKEEPER)
                    dir *= 1.4f;
                break;
            case CharacterActions.kickTypes.HEAD:
                KickBallSound();
                dir *= GetStats().kickHead * 1.5f;
                dir += Vector3.up * GetStats().kickHeadAngle;// * force;
                break;
            case CharacterActions.kickTypes.CHILENA:
                KickBallSound();
                dir *= GetStats().kickChilena * (Random.Range(17, 32) / 10);
                dir += Vector3.up * GetStats().kickChilenaAngle*(Random.Range(17,28)/10);// * force;
                break;
            case CharacterActions.kickTypes.KICK_TO_GOAL:
                KickBallSound();
                dir *= GetStats().kickHard * Random.Range(2, 3.5f);
               
                if (character != null && character.type == Character.types.GOALKEEPER)
                    dir += Vector3.up * GetStats().kickHardAngle * 2;
                else
                    dir += Vector3.up * GetStats().kickHardAngle * (force/1.35f);
                break;
            case CharacterActions.kickTypes.CENTRO:
                KickBallSound();
                dir *= GetStats().kickCentro * 1.5f;
                dir += Vector3.up * GetStats().kickCentroAngle * force;
                break;
        }
        rb.velocity = Vector3.zero;
        rb.AddForce(dir);

        if (character != null)
            character.SetCollidersOff(GetStats().freeze_by_kick);

        Events.OnBallKicked(kickType, forceForce, character);
        character = null;
    }
    int kick_id = 1;
    void KickBallSound()
    {
        Events.PlaySound("common", "kick" + kick_id, false);
        kick_id++;
        if (kick_id > 4) kick_id = 1;
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
    public void PaseTo(Character character)
    {
        ballAI.Pase(character);
    }

    

    Transform customContainer;
    private void Update()
    {
        if (customContainer == null)
            return;

        Vector3 dest = customContainer.transform.position + (customContainer.forward * offsetForward);
        dest.y += catchedOffsetY;
        transform.localEulerAngles = customContainer.transform.eulerAngles;
        transform.localPosition = Vector3.Lerp(transform.localPosition, dest, Time.deltaTime* smooth);
    }
    public void Catched(Transform customContainer)
    {
        this.customContainer = customContainer;
        ForcePosition(customContainer);
    }
    public void ForcePosition(Transform customContainer)
    {
        transform.localPosition = customContainer.transform.position + new Vector3(0, catchedOffsetY, 0);
        rb.velocity = Vector3.zero;
        transform.localEulerAngles = customContainer.transform.localEulerAngles;
    }
}
