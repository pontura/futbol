﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VoicesManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource audioSourceComentarios;

    
    public AudioClip[] nums;
    public AudioClip a;

    public AudioClip[] intro_salen;
    public AudioClip[] intro_sale_referi;

    public AudioClip[] game_start;
    public AudioClip[] comentario_gol;
    public AudioClip[] comentario_gol_en_contra;
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
    public AudioClip[] arquero_hands;
    public AudioClip[] arquero_saca;
    public AudioClip[] gol_generico;
    public AudioClip[] gol_en_contra;
    public AudioClip[] pide_comentario;
    public AudioClip[] responde_comentario_gol;   
    Character character;

    static VoicesManager mInstance = null;
    public static VoicesManager Instance
    {
        get
        {
            return mInstance;
        }
    }
    void Awake()
    {
        if (!mInstance)
        {
            mInstance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }
    void Start()
    {
        Events.CharacterCatchBall += CharacterCatchBall;
        Events.KickToGoal += KickToGoal;
        Events.OnBallKicked += OnBallKicked;
        Events.OnGoal += OnGoal;
        Events.OnIntroSound += OnIntroSound;  
    }
    void OnDestroy()
    {
        Events.CharacterCatchBall -= CharacterCatchBall;
        Events.OnBallKicked -= OnBallKicked;
        Events.KickToGoal -= KickToGoal;
        Events.OnGoal -= OnGoal;
        Events.OnIntroSound -= OnIntroSound;
    }
    public void SayResults()
    {
        Vector2 score = UIMain.Instance.GetScore();
        if (score == Vector2.zero)
            PlayAudios(new AudioClip[] { GetRandomAudioClip(game_start) });
        else if (score.x < 11 && score.y < 11)
        {
            AudioClip score1 = nums[(int)score.x];
            AudioClip score2 = nums[(int)score.y];
            PlayAudios(new AudioClip[] { score1, a, score2 }, null);
        }
    }
    void OnIntroSound(int id, Character character)
    {
        AudioClip nameClip = null;
        if(character != null)
            nameClip = GetRandomAudioClip(character.dataSources.audio_names);
        if (id == 1)
            PlayAudios(new AudioClip[] { GetRandomAudioClip(intro_salen) });
        else if (id == 2)
            PlayAudios(new AudioClip[] { GetRandomAudioClip(intro_sale_referi), nameClip });
        else
            PlayAudios(new AudioClip[] { nameClip });

    }
    void OnGoal(int teamID, Character character)
    {
        Reset();
        StopAllCoroutines();
        if (teamID == character.teamID)
        {
            if (character.dataSources.audio_goal.Length > 0)
            {
                PlayAudios(new AudioClip[] {
                    GetRandomAudioClip(character.dataSources.audio_goal),
                    GetRandomAudioClip(pide_comentario)
                }, SayComentarioGoal);
            }
            else
            {
                PlayAudios(new AudioClip[] {
                    GetRandomAudioClip(gol_generico),
                    GetRandomAudioClip(character.dataSources.audio_names),
                    GetRandomAudioClip(pide_comentario)                    
                }, SayComentarioGoal);                
            }
        }
        else
        {
            PlayAudios(new AudioClip[] {
                GetRandomAudioClip(gol_en_contra),
                GetRandomAudioClip(character.dataSources.audio_names),
                GetRandomAudioClip(pide_comentario)
            }, SayComentarioGoalEnContra);
        }
    }
    void SayComentarioGoal()
    {
        PlayAudiosComentarista(new AudioClip[] { GetRandomAudioClip(comentario_gol) }, SayGoalEnd);
    }
    void SayComentarioGoalEnContra()
    {
        PlayAudiosComentarista(new AudioClip[] { GetRandomAudioClip(comentario_gol_en_contra) }, SayGoalEnd);
    }
    void SayGoalEnd()
    {
        PlayAudios(new AudioClip[] {
                    GetRandomAudioClip(responde_comentario_gol)
                }, Events.OnRestartGame);
    }
    private void Reset()
    {
        sigueID = 0;
        CancelInvoke();
    }
    void OnBallKicked(CharacterActions.kickTypes kickType, float forceForce, Character character)
    {
        Reset();
        switch (kickType)
        {
            case CharacterActions.kickTypes.DESPEJE_GOALKEEPER:
                PlayAudios(new AudioClip[] { GetRandomAudioClip(arquero_hands) });
                break;
            case CharacterActions.kickTypes.CHILENA:
                PlayAudios(new AudioClip[] { GetRandomAudioClip(chilena) });
                break;
            case CharacterActions.kickTypes.HARD:
                if (character != null && character.isGoldKeeper)
                {
                    AudioClip characterName = GetRandomAudioClip(character.dataSources.audio_names);
                    PlayAudios(new AudioClip[] { characterName, GetRandomAudioClip(arquero_saca) });
                }else
                    PlayAudios(new AudioClip[] { GetRandomAudioClip(le_pega_al_arco) });
                break;
            case CharacterActions.kickTypes.BALOON:
                if (character != null && character.isGoldKeeper)
                {
                    AudioClip characterName = GetRandomAudioClip(character.dataSources.audio_names);
                    PlayAudios(new AudioClip[] { characterName, GetRandomAudioClip(arquero_saca) });
                } else
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
        this.character = character;
        
        if (character.isGoldKeeper)
            Invoke("SaySigue", 3);
        else
            Invoke("SaySigue", 2);

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
    void PlayAudios(AudioClip[] audioClips, System.Action OnDone = null)
    {
        StartCoroutine( WaitForSound(audioClips, OnDone, audioSource) );
    }
    void PlayAudiosComentarista(AudioClip[] audioClips, System.Action OnDone = null)
    {
        StartCoroutine(WaitForSound(audioClips, OnDone, audioSourceComentarios));
    }
    public IEnumerator WaitForSound(AudioClip[] audioClips, System.Action OnDone, AudioSource aSource)
    {
        
        foreach (AudioClip audioClip in audioClips)
        {
            
            aSource.clip = audioClip;
            aSource.Play();
            yield return new WaitUntil(() => aSource.isPlaying == false);
        }
        if(OnDone != null)
        {
            Debug.Log("OnDone " + OnDone);
            OnDone();
        }
            
    }
    
}
