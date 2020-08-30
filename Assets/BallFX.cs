using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallFX : MonoBehaviour
{
    public ParticleSystem kickParticles;
    public float rate = 500;
    public float speedDecrease = 10;
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
    void OnBallKicked()
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
            yield return new WaitForSeconds(0.025f);
            value -= speedDecrease;
            emissionModule.rateOverTime = value;
        }
        emissionModule.rateOverTime = 0;
    }
}
