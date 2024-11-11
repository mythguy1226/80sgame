using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashTransition : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Used for length of splash screen")]
    private VideoClip clip;

    [SerializeField]
    [Tooltip("Used for length of splash screen")]
    private VideoClip magicSpellStudiosClip;

    public VideoPlayer clipPlayer;

    [Tooltip("Time of transition in seconds")]
    public float transitionBuffer = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TransitionToStudioSplash());
    }

    IEnumerator TransitionToStudioSplash()
    {
        yield return new WaitForSeconds((float)magicSpellStudiosClip.length + transitionBuffer);
        clipPlayer.Play();
        StartCoroutine(TransitionToTitleScreen());
    }

    IEnumerator TransitionToTitleScreen()
    {
        yield return new WaitForSeconds((float)clip.length + transitionBuffer);
        SceneManager.LoadScene(2);
    }
}
