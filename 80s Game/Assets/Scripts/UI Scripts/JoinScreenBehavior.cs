using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinScreenBehavior : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(3);
    }
}