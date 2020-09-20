﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public AudioSourceManager[] all;
    [Serializable]
    public class AudioSourceManager
    {
        public string sourceName;
        [HideInInspector] public AudioSource audioSource;
        public float volume = 1;
    }
    void Start()
    {
        Events.PlaySound += PlaySound;
        foreach (AudioSourceManager m in all)
        {
            m.audioSource = gameObject.AddComponent<AudioSource>();
            m.audioSource.volume = m.volume;
        }
    }

    void PlaySound(string sourceName, string audioName, bool loop)
    {
        foreach(AudioSourceManager m in all)
        {
            if(m.sourceName == sourceName)
            {
                m.audioSource.clip = Resources.Load<AudioClip>("Audio/" + audioName) as AudioClip;
                m.audioSource.Play();
                m.audioSource.loop = loop;
            }
        }
    }
}
