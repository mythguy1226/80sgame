using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //singleton implementation adapted from an online tutorial: https://www.youtube.com/watch?v=tEsuLTpz_DU
    public static SoundManager Instance;

    //AudioSources for sound playback.
    public AudioSource interruptSource;     //audio source for sounds which cancel each other out
    public AudioSource continuousSource;    //audio source for sounds which cannot be interrupted
    [SerializeField] AudioSource[] _musicLoopSources;     //audio sources for looping music track(s) seamlessly

    //NOTE: non-looping music like between-round and game end themes should use PlayNonloopMusic()!

    //param(s) for music volume
    private float _musicVolume = 1f;    //volume for music. Adjusted in SettingsManager via the accessor below.
    public float MusicVolume            //accessor for the above _musicVolume variable.
    {
        get { return _musicVolume; }
        //setter auto-updates the _musicLoopSources' volume whenever music volume is adjusted.
        //saves unnecessary assignments in update.
        set 
        {
            _musicVolume = value;
            for(int i = 0; i < _musicLoopSources.Length; i++)
            {
                if (_musicLoopSources[i] != null) _musicLoopSources[i].volume = value;
            }
        }
    }

    public float sfxVolume = 1f; //param for sfx volume

    //fields for music loops
    //music loop implementation taken from the unity online manual: https://docs.unity3d.com/2021.3/Documentation/ScriptReference/AudioSource.PlayScheduled.html

    private bool _bIsMusicPlaying;        //whether or not music is currently playing
    public bool BIsMusicPlaying {  get { return _bIsMusicPlaying; } }


    private MusicTrack _musicLoopClip;        //MusicTrack to loop

    private double _nextEventTime;           //time at which next loop plays
    private int _flip = 0;                   //which loop source is being used?

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
        _nextEventTime = AudioSettings.dspTime + 0.5f;
    }

    /// <summary>
    /// Plays an AudioClip once using sfxVolume. Use this for sounds that should not layer on top of
    /// one another (e.g. stunning sounds).
    /// </summary>
    /// <param name="clip">The AudioClip to be played.</param>
    public void PlaySoundInterrupt(AudioClip clip)
    {
        if (interruptSource.isPlaying)
        {
            interruptSource.Stop(); //interrupts current sound playing through interruptSource, to avoid sounds stacking on top of one another
        }
        interruptSource.PlayOneShot(clip, sfxVolume);

    }
    /// <summary>
    /// Plays an AudioClip once using sfxVolume. Use this for sounds that can be allowed to 
    /// layer on top of one another, e.g. shoot sounds.
    /// </summary>
    /// 
    /// <param name="clip">AudioClip to be played.</param>
    public void PlaySoundContinuous(AudioClip clip)
    { 
        continuousSource.PlayOneShot(clip, sfxVolume);
    }


    /// <summary>
    /// Plays a designated MusicTrack once, respecting the current Music Volume setting.
    /// Note that this uses a <b>MusicTrack</b>, <i>not an AudioClip</i>.
    /// </summary>
    /// 
    /// <param name="clip">The MusicTrack to be played.</param>
    public void PlayNonloopMusic(MusicTrack clip)
    {
        if (clip != null && clip.Clip != null)
        {
            //if (_bIsMusicPlaying) //outdated, but keeping here in case we end up needing it.
            //{
            //    foreach (AudioSource aS in _musicLoopSources)
            //    {
            //        aS.volume *= 0.2f; //sets the volume of the _musicLoopSources to 20%.
            //    }
            //}
            continuousSource.PlayOneShot(clip.Clip, _musicVolume);
            //StartCoroutine(NonloopUnmute(clip)); //starts a coroutine to unmute the _musicLoopSources
        }
    }
    /// <summary>
    /// Coroutine to time unmuting the _musicLoopSources.
    /// Should <i>only</i> be called by methods that mute and unmute the BGM.
    /// </summary>
    /// 
    /// <param name="clip">The clip for whose duration the _musicLoopSources will be muted.</param>
    /// 
    /// <returns>IEnumerator, as this is a coroutine.</returns>
    IEnumerator NonloopUnmute(MusicTrack clip)
    {
        yield return new WaitForSeconds(clip.Clip.length - (float)(clip.EndOffset + clip.StartOffset));
        foreach (AudioSource aS in _musicLoopSources)
        {
            aS.volume = _musicVolume;
        }
    }
    /// <summary>
    /// Immediately stop whatever looping track is playing.
    /// </summary>
    public void StopMusicLoop()
    {
        for(int i = 0; i < _musicLoopSources.Length; i++)
        {
            if (_musicLoopSources[i] == null) continue;
            _musicLoopSources[i].Stop();
        }

        _bIsMusicPlaying = false;
    }


    /// <summary>
    /// Sets the MusicTrack to loop, and schedules the initial play time.
    /// This does <i>not</i> play the music.
    /// </summary>
    /// 
    /// <param name="clip">The MusicTrack to be looped.</param>
    /// 
    /// <param name="introEndDelay">
    /// Delay after which clip playback should start. Used for scheduling the initial loop play time.
    /// Defaults to zero.
    /// Set this up if the looping clip <i>immediately</i> follows an intro segment.
    /// </param>
    public void SetMusicToLoop(MusicTrack clip, double introEndDelay = 0)
    {
        //probably dangerous, but shouldn't need to be triggered anyway
        if (_musicLoopSources == null || _musicLoopSources.Length == 0)
        {

            _musicLoopSources = new AudioSource[2];
            _musicLoopSources[0] = new AudioSource();
            _musicLoopSources[0].volume = _musicVolume;
            _musicLoopSources[1] = new AudioSource();
            _musicLoopSources[1].volume = _musicVolume;
        }
        else
        {
            if (_musicLoopSources[0] == null)
            {
                _musicLoopSources[0] = new AudioSource();
                _musicLoopSources[0].volume = _musicVolume;
            }
            if (_musicLoopSources[1] == null)
            {
                _musicLoopSources[1] = new AudioSource();
                _musicLoopSources[1].volume = _musicVolume;
            }
        }
        //stop whatever might be looping before scheduling another song to loop
        StopMusicLoop();
        if (clip != null)
        {
            //duplicates clip and overwrites _musicLoopClip.
            _musicLoopClip = new MusicTrack(clip);

            //schedules the music to start looping after 1 second or after the intro is finished, whichever would be longer.
            //defaults to 0, as not every looping song will have an intro (e.g. the Title theme).
            _nextEventTime = AudioSettings.dspTime + (introEndDelay > 1.0? introEndDelay:1.0);

            //congratulations, music is now playing if it wasn't already.
            _bIsMusicPlaying = true;
        }
        else return;
    }

    //<i>~unabashedly~</i> mostly copied from my (~QP) Spring 2024 IGME670 IME Lab 9 implementation.
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
        if (time + 1.0 > _nextEventTime) //if the next loop will play within the next second...
        {
            //sets the currently free _musicLoopSource to play the looping clip at _nextEventTime.
            _musicLoopSources[_flip].clip = _musicLoopClip.Clip;
            _musicLoopSources[_flip].PlayScheduled(_nextEventTime);

            //schedule next event based on the Start and End Offsets of the clip.
            _nextEventTime += _musicLoopClip.Clip.length - (_musicLoopClip.StartOffset + _musicLoopClip.EndOffset);

            //invert flip so the next loop will start playing on a vacant AudioSource.
            _flip = 1 - _flip;
        }
    }
}
