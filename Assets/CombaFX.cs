using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombaFX : MonoBehaviour
{
    Character character;
    float initial_Z;
    Rigidbody rb;
    public float force = 100;

    void Start()
    {
        Events.OnBallKicked += OnBallKicked;
        rb = GetComponent<Rigidbody>();
    }
    void OnDestroy()
    {
        Events.OnBallKicked -= OnBallKicked;
    }
    void OnBallKicked(CharacterActions.kickTypes type, float force, Character character )
    {
        print(type);
        if (character == null) return;
        if ((type == CharacterActions.kickTypes.HARD || type == CharacterActions.kickTypes.KICK_TO_GOAL) && character.isBeingControlled)
            Init(character);
    }
    private void Init(Character character)
    {
        this.character = character;
        initial_Z = character.transform.position.z;
    }
    public void Reset()
    {
        character = null;
    }
    void Update()
    {
        if (character == null) return;
        if (character.transform.position.z == initial_Z) return;

        float direction = initial_Z - character.transform.position.z;

        rb.AddForce(-Vector3.forward * force * direction);
        initial_Z = character.transform.position.z;
    }
}
