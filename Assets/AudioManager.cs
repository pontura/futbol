using System.Collections;
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
        public AudioClip[] clips;
    }
    void Start()
    {
        Events.PlaySound += PlaySound;
        foreach (AudioSourceManager m in all)
        {
            m.audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void PlaySound(string sourceName, string audioName, bool loop)
    {
        foreach(AudioSourceManager m in all)
        {
            if(m.sourceName == sourceName)
            {
                foreach (AudioClip ac in m.clips)
                {
                    if (ac.name == audioName)
                    {
                        m.audioSource.clip = ac;
                        m.audioSource.Play();
                        m.audioSource.loop = loop;
                    }
                }
            }
        }
    }
}
