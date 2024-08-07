using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //singleton implementation adapted from an online tutorial: https://www.youtube.com/watch?v=tEsuLTpz_DU
    public static SoundManager Instance;

    public AudioSource interruptSource;     //audio source for sounds which cancel each other out
    public AudioSource continuousSource;    //audio source for sounds which cannot be interrupted
    [SerializeField] AudioSource[] musicLoopSources;     //audio sources for looping music track(s) seamlessly

    //NOTE: non-looping music like between-round and game end themes should use PlayNonloopMusic()!

    //param(s) for music volume
    private float musicvolume = 1f;
    public float MusicVolume
    {
        get { return musicvolume; }
        set 
        {
            musicvolume = value;
            for(int i = 0; i < musicLoopSources.Length; i++)
            {
                if (musicLoopSources[i] != null) musicLoopSources[i].volume = value;
            }
        }
    }

    //param for sfx volume
    public float sfxVolume = 1f;

    //fields for music loops
    //music loop implementation taken from the unity online manual: https://docs.unity3d.com/2021.3/Documentation/ScriptReference/AudioSource.PlayScheduled.html
    private bool isMusicPlaying;        //whether or not music is currently playing
    public bool IsMusicPlaying {  get { return isMusicPlaying; } }

    private AudioClip musicLoopClip;        //AudioClip to loop

    private double nextEventTime;           //time to next loop plays
    private int flip = 0;                   //which loop source is being used?

    //fields for non-looping music tracks
    //TODO: add functionality to mute the currently looping track temporarily while jingle plays (see also PlayNonloopMusic())

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        isMusicPlaying = true;
        nextEventTime = AudioSettings.dspTime + 0.5f;
    }

    public void PlaySoundInterrupt(AudioClip clip)
    {
        if (interruptSource.isPlaying)
        {
            interruptSource.Stop(); //interrupts current sound playing through interruptSource, to avoid sounds stacking on top of one another
        }
        interruptSource.PlayOneShot(clip, sfxVolume);

    }

    public void PlaySoundContinuous(AudioClip clip)
    { 
        continuousSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayNonloopMusic(AudioClip clip)
    {
        if(isMusicPlaying) 
        {
            //TODO: add functionality to mute the currently looping track temporarily while jingle plays
        }
        Debug.Log("Playing one-shot jingle!");
        continuousSource.PlayOneShot(clip, musicvolume);
    }

    //immediately stop whatever looping track is playing
    public void StopMusicLoop()
    {
        for(int i = 0; i < musicLoopSources.Length; i++)
        {
            if (musicLoopSources[i] == null) continue;
            musicLoopSources[i].Stop();
        }

        isMusicPlaying = false;
    }

    public void SetMusicToLoop(AudioClip clip) //sets up music to loop. This DOES NOT PLAY the music
    {
        //probably dangerous, but shouldn't need to be triggered anyway
        if (musicLoopSources == null || musicLoopSources.Length == 0)
        {

            musicLoopSources = new AudioSource[2];
            musicLoopSources[0] = new AudioSource();
            musicLoopSources[0].volume = musicvolume;
            musicLoopSources[1] = new AudioSource();
            musicLoopSources[1].volume = musicvolume;
        }
        else
        {
            if (musicLoopSources[0] == null)
            {
                musicLoopSources[0] = new AudioSource();
                musicLoopSources[0].volume = musicvolume;
            }
            if (musicLoopSources[1] == null)
            {
                musicLoopSources[1] = new AudioSource();
                musicLoopSources[1].volume = musicvolume;
            }
        }

        StopMusicLoop();
        if (clip != null)
        {
            musicLoopClip = clip;

            nextEventTime = AudioSettings.dspTime + 1.0f;

            isMusicPlaying = true;
        }
        else
        {

            return;
        }
    }

    //unabashedly copied from my (~QP) IGME670 (Spr 2024) IME Lab 9 implementation
    private void Update()
    {
        if (!isMusicPlaying) //update does nothing if music is not currently set to play
        {

            return; 
        }

        double time = AudioSettings.dspTime;


        if(musicLoopClip == null) //can't play music if there *is* no music
        {
            isMusicPlaying = false;
            return;
        }
        if (time + 1.0f > nextEventTime) //if the next loop will play within the next second
        {
            musicLoopSources[flip].clip = musicLoopClip;
            musicLoopSources[flip].PlayScheduled(nextEventTime);

            nextEventTime += musicLoopClip.length;

            flip = 1 - flip; //just used the vanilla version for simplicity ;u; //(flip + 1) % (musicLoopSources.Length); //length should only ever be 2 (indices 0 or 1), but here's a failsafe in case that isn't the case for some reason.
        }
    }
}
