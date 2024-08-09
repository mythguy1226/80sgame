using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashTransition : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Used for length of splash screen")]
    private VideoClip clip;

    [Tooltip("Time of transition in seconds")]
    public float transitionTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TransitionToTitleScreen());
    }

    IEnumerator TransitionToTitleScreen()
    {
        yield return new WaitForSeconds((float)clip.length);
        SceneManager.LoadScene(1);
    }
}
