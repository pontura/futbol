using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VoicesManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] laDomina;
    public AudioClip[] sigue;
    public AudioClip[] sigue2;
    public AudioClip[] ataja;
    public AudioClip[] le_pega_al_arco;
    public AudioClip[] chilena;
    public AudioClip[] volea;
    public AudioClip[] globito;
    public AudioClip[] pase;
    public AudioClip[] cabeza;
    public AudioClip[] arquero_espera;
    Character character;
    void Start()
    {
        Events.CharacterCatchBall += CharacterCatchBall;
        Events.KickToGoal += KickToGoal;
        Events.OnBallKicked += OnBallKicked;
        Events.OnGoal += OnGoal;
    }
    void OnDestroy()
    {
        Events.CharacterCatchBall += CharacterCatchBall;
        Events.KickToGoal -= KickToGoal;
        Events.OnGoal -= OnGoal;
    }
    void OnGoal(int teamID, Character character)
    {
        Reset();
        StopAllCoroutines();
        if (character.dataSources.audio_goal.Length > 0)
        {            
            PlayAudios(new AudioClip[] { GetRandomAudioClip(character.dataSources.audio_goal) });
        }
    }
    private void Reset()
    {
        sigueID = 0;
        CancelInvoke();
    }
    void OnBallKicked(CharacterActions.kickTypes kickType, float forceForce)
    {
        Reset();
        switch (kickType)
        {
            case CharacterActions.kickTypes.CHILENA:
                PlayAudios(new AudioClip[] { GetRandomAudioClip(chilena) });
                break;
            case CharacterActions.kickTypes.HARD:
                PlayAudios(new AudioClip[] { GetRandomAudioClip(le_pega_al_arco) });
                break;
            case CharacterActions.kickTypes.BALOON:
                PlayAudios(new AudioClip[] { GetRandomAudioClip(globito) });
                break;
            case CharacterActions.kickTypes.SOFT:
                PlayAudios(new AudioClip[] { GetRandomAudioClip(pase) });
                break;
            case CharacterActions.kickTypes.HEAD:
                PlayAudios(new AudioClip[] { GetRandomAudioClip(cabeza) });
                break;
            case CharacterActions.kickTypes.KICK_TO_GOAL:
                PlayAudios(new AudioClip[] { GetRandomAudioClip(le_pega_al_arco) });
                break;
        }
    }
    void KickToGoal()
    {
        Reset();
        PlayAudios(new AudioClip[] { GetRandomAudioClip(volea) }); 
    }

    int sigueID;
    void SaySigue()
    {
        if (character != null)
        {
            if (character.isGoldKeeper)
            {
                PlayAudios(new AudioClip[] { GetRandomAudioClip(arquero_espera) });
            }
            else
            {
                sigueID++;

                AudioClip characterName = GetRandomAudioClip(character.dataSources.audio_names);
                if (sigueID == 1)
                    PlayAudios(new AudioClip[] { GetRandomAudioClip(sigue), characterName });
                else
                    PlayAudios(new AudioClip[] { GetRandomAudioClip(sigue2), characterName });

                Invoke("SaySigue", 2);
            }
           
        }        
    }
    void CharacterCatchBall(Character character)
    {
        Reset();
        Invoke("SaySigue", 2);
       this.character = character;
        if (character.dataSources.audio_names != null && character.dataSources.audio_names.Length > 0)
        {
            StopAllCoroutines();
            int rand = UnityEngine.Random.Range(0, 10);
            AudioClip characterName = GetRandomAudioClip(character.dataSources.audio_names);
            if(character.isGoldKeeper)
            {                
                if (rand > 2)
                    PlayAudios(new AudioClip[] {  GetRandomAudioClip(ataja),   characterName  });
                else
                    PlayAudios(new AudioClip[] { characterName });
                return;
            }

            if (rand > 5)
                PlayAudios(new AudioClip[] {   GetRandomAudioClip(laDomina), characterName   });
            else
                PlayAudios(new AudioClip[] {  characterName  });

        } else
        {
            Debug.Log("No grabaron audio para el nombre de: " + character.characterID);
        }
    }
    AudioClip GetRandomAudioClip(AudioClip[] audioClips)
    {
       return audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
    }
    void PlayAudios(AudioClip[] audioClips)
    {
        StartCoroutine( WaitForSound(audioClips) );
    }
    public IEnumerator WaitForSound(AudioClip[] audioClips)
    {
        foreach (AudioClip audioClip in audioClips)
        {
            yield return new WaitUntil(() => audioSource.isPlaying == false);
            audioSource.clip = audioClip;
            audioSource.Play();
        }

    }
}
