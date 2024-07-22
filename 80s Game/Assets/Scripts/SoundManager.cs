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
    private float _musicvolume = 1f;
    public float MusicVolume
    {
        get { return _musicvolume; }
        set 
        {
            _musicvolume = value;
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
    private bool _bIsMusicPlaying;        //whether or not music is currently playing
    public bool bIsMusicPlaying {  get { return _bIsMusicPlaying; } }

    private MusicTrack _musicLoopClip;        //MusicTrack to loop
    //private double _musicLoopStartOffset;
    //private double _musicLoopEndOffset;

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
        _bIsMusicPlaying = true;
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
    /// <summary>
    /// Plays a designated MusicTrack once, respecting the current Music Volume setting.
    /// Also mutes both musicLoopSources while this 
    /// </summary>
    /// <param name="clip">The MusicTrack to be played.</param>
    public void PlayNonloopMusic(MusicTrack clip)
    {
        if (_bIsMusicPlaying)
        {
            foreach (AudioSource aS in musicLoopSources)
            {
                aS.volume = 0;
            }
        }
        continuousSource.PlayOneShot(clip.Clip, _musicvolume);
        StartCoroutine(NonloopUnmute(clip));
    }
    IEnumerator NonloopUnmute(MusicTrack clip)
    {
        yield return new WaitForSeconds(clip.Clip.length - (float)(clip.EndOffset + clip.StartOffset));
        foreach (AudioSource aS in musicLoopSources)
        {
            aS.volume = _musicvolume;
        }
    }

    /// <summary>
    /// Immediately stop whatever looping track is playing.
    /// </summary>
    public void StopMusicLoop()
    {
        for(int i = 0; i < musicLoopSources.Length; i++)
        {
            if (musicLoopSources[i] == null) continue;
            musicLoopSources[i].Stop();
        }

        _bIsMusicPlaying = false;
    }

    public void SetMusicToLoop(MusicTrack clip, double introEndDelay = 0) //sets up music to loop. This DOES NOT PLAY the music
    {
        //probably dangerous, but shouldn't need to be triggered anyway
        if (musicLoopSources == null || musicLoopSources.Length == 0)
        {

            musicLoopSources = new AudioSource[2];
            musicLoopSources[0] = new AudioSource();
            musicLoopSources[0].volume = _musicvolume;
            musicLoopSources[1] = new AudioSource();
            musicLoopSources[1].volume = _musicvolume;
        }
        else
        {
            if (musicLoopSources[0] == null)
            {
                musicLoopSources[0] = new AudioSource();
                musicLoopSources[0].volume = _musicvolume;
            }
            if (musicLoopSources[1] == null)
            {
                musicLoopSources[1] = new AudioSource();
                musicLoopSources[1].volume = _musicvolume;
            }
        }

        StopMusicLoop();
        if (clip != null)
        {
            _musicLoopClip = new MusicTrack(clip);
            Debug.Log("Clip length: " + _musicLoopClip.Clip.length);

            nextEventTime = AudioSettings.dspTime + (introEndDelay > 1.0? introEndDelay:1.0);

            _bIsMusicPlaying = true;
        }
        else
        {

            return;
        }
    }

    //unabashedly copied from my (~QP) IGME670 (Spr 2024) IME Lab 9 implementation
    private void Update()
    {
        if (!_bIsMusicPlaying) //update does nothing if music is not currently set to play
        {

            return; 
        }

        double time = AudioSettings.dspTime;


        if(_musicLoopClip == null) //can't play music if there *is* no music
        {
            _bIsMusicPlaying = false;
            return;
        }
        if (time + 1.0 > nextEventTime) //if the next loop will play within the next second
        {
            musicLoopSources[flip].clip = _musicLoopClip.Clip;
            musicLoopSources[flip].PlayScheduled(nextEventTime);

            //schedule next event based on the Start and End Offsets of the clip.
            nextEventTime += _musicLoopClip.Clip.length - (_musicLoopClip.StartOffset + _musicLoopClip.EndOffset);

            flip = 1 - flip;
        }
    }
}
