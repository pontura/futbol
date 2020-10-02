using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallFX : MonoBehaviour
{
    public ParticleSystem kickParticles;
    public float rate = 2000;
    public float speedDecrease = 40;
    ParticleSystem.EmissionModule emissionModule;
    float value;

    void Start()
    {
        emissionModule = kickParticles.emission;
        Events.OnBallKicked += OnBallKicked;
    }
    void OnDestroy()
    {
        Events.OnBallKicked -= OnBallKicked;
    }   
    void OnBallKicked(CharacterActions.kickTypes kickType, float forceForce, Character character)
    {
        value += rate;
        emissionModule.rateOverTime = value;
        kickParticles.Play();
        StartCoroutine(TurnOff());
    }
    IEnumerator TurnOff()
    {
        while (value > 0)
        {
            yield return new WaitForSeconds(0.01f);
            value -= speedDecrease;
            emissionModule.rateOverTime = value;
        }
        emissionModule.rateOverTime = 0;
    }
}
