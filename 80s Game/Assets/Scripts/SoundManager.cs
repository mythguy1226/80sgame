using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //singleton implementation adapted from an online tutorial: https://www.youtube.com/watch?v=tEsuLTpz_DU
    public static SoundManager Instance;

    public AudioSource interruptSource; //audio source for sounds which cancel each other out
    public AudioSource continuousSource; //audio source for sounds which cannot be interrupted

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void PlaySoundInterrupt(AudioClip clip)
    {
        if (interruptSource.isPlaying) 
        {
            interruptSource.Stop();
        }
        interruptSource.PlayOneShot(clip);
    }

    public void PlaySoundContinuous(AudioClip clip)
    { 
        continuousSource.PlayOneShot(clip);
    }
}
