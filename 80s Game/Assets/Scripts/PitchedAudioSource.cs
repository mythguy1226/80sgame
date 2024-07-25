using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PitchedAudioSource : MonoBehaviour
{
    AudioSource audioSource;
    Coroutine playingCoroutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            gameObject.AddComponent<AudioSource>();
        }
    }

    public void Play()
    {
        if(playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);
        }

        audioSource.Play();
        playingCoroutine = StartCoroutine(WaitForSoundToEnd());
    }

    IEnumerator WaitForSoundToEnd()
    {
        yield return new WaitWhile(() => audioSource.isPlaying);
        SoundManager.Instance.ReturnToPool(this);
    }

    public void Stop()
    {
        if(playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);
            playingCoroutine = null;
        }

        audioSource.Stop();
        SoundManager.Instance.ReturnToPool(this);
    }

    public PitchedAudioSource WithClip(AudioClip clip)
    {
        audioSource.clip = clip;
        return this;
    }

    public PitchedAudioSource WithVolume(float volume)
    {
        audioSource.volume = volume;
        return this;
    }

    public PitchedAudioSource WithPitch(float min = 0.9f, float max = 1.1f)
    {
        audioSource.pitch = Random.Range(min, max);
        return this;
    }
}
