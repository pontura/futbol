using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VoicesManager : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        Events.CharacterCatchBall += CharacterCatchBall;
    }
    void OnDestroy()
    {
        Events.CharacterCatchBall += CharacterCatchBall;
    }
    void CharacterCatchBall(Character character)
    {
        if (character.dataSources.audio_names != null && character.dataSources.audio_names.Length > 0)
        {
            audioSource.clip = character.dataSources.audio_names[0];
            audioSource.Play();
        } else
        {
            Debug.Log("No grabaron audio para el nombre de: " + character.characterID);
        }
    }
}
