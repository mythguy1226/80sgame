using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //singleton implementation adapted from an online tutorial: https://www.youtube.com/watch?v=tEsuLTpz_DU
    public static SoundManager Instance;

    public AudioSource interruptSource; //audio source for sounds which cancel each other out
    public AudioSource continuousSource; //audio source for sounds which cannot be interrupted

    public float volume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    //vol is a volume scale,
    public void PlaySoundInterrupt(AudioClip clip, float vol = 1f)
    {
        if (interruptSource.isPlaying)
        {
            interruptSource.Stop(); //interrupts current sound playing through interruptSource, to avoid sounds stacking on top of one another
        }
        interruptSource.PlayOneShot(clip, volume);
        //Debug.Log(clip);
    }

    public void PlaySoundContinuous(AudioClip clip, float vol = 1f)
    { 
        continuousSource.PlayOneShot(clip, volume);
        //Debug.Log(clip);
    }
}
