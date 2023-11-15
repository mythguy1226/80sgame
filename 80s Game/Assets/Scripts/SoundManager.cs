using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //singleton implementation adapted from an online tutorial: https://www.youtube.com/watch?v=tEsuLTpz_DU
    public static SoundManager Instance;

    public AudioSource audio; //uses a single audio source

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void PlaySound(AudioClip clip)
    {
        audio.PlayOneShot(clip);
    }
}
