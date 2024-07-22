using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MusicTrack")]
/// <summary>
/// Object containing an AudioClip for a Music track, as well as the Start offset and End offset.
/// </summary>
public class MusicTrack : ScriptableObject
{
    /*
     * A few tracks have short empty spaces at the beginning and/or end,
     * which need to be accounted for when scheduling them to play.
    */

    /// <summary>
    /// The amount of time it takes, <i>in seconds</i>, for the 
    /// music to start after the file begins playing.
    /// </summary>
    [SerializeField] double _startOffset;
    public double StartOffset
    {
        get { return _startOffset; }
        private set { _startOffset = value; }
    }

    /// <summary>
    /// The amount of time, <i>in seconds</i>, between the end
    /// of the clip's music and the end of its actual duration.
    /// </summary>
    [SerializeField] double _endOffset;
    public double EndOffset
    { 
        get { return _endOffset; } 
        private set { _endOffset = value; }
    }

    /// <summary>
    /// The music track.
    /// </summary>
    [SerializeField] AudioClip _clip;
    public AudioClip Clip
    {
        get { return _clip; }
        private set { _clip = value; }
    }



    /// <summary>
    /// Constructor for clips that have no offsets.
    /// </summary>
    /// <param name="clip">The music track.</param>
    public MusicTrack(AudioClip clip)
    {
        StartOffset = 0;
        EndOffset = 0;
        Clip = clip;
    }

    /// <summary>
    /// Constructor for clips that need offsets to play properly.
    /// </summary>
    /// <param name="startOffset">
    /// The amount of time it takes, <i>in seconds</i>, for the 
    /// music to start after the file begins playing.
    /// </param>
    /// 
    /// <param name="endOffset">
    /// The amount of time, <i>in seconds</i>, between the end
    /// of the clip's music and the end of its actual duration.
    /// </param>
    /// 
    /// <param name="clip">The music track.</param>
    public MusicTrack(double startOffset, double endOffset, AudioClip clip)
    {
        StartOffset = startOffset;
        EndOffset = endOffset;
        Clip = clip;
    }

    /// <summary>
    /// Constructor for copying a MusicTrack.
    /// </summary>
    /// <param name="musicClip">The MusicTrack to be copied.</param>
    public MusicTrack(MusicTrack musicClip)
    {
        StartOffset = musicClip.StartOffset;
        EndOffset = musicClip.EndOffset;
        Clip = musicClip.Clip;
    }

    public override string ToString()
    {
        return "Clip: " + _clip.ToString()
            + " \nStartOffset: " + _startOffset
            + " \nEndOffset: " + _endOffset;
    }
}
