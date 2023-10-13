using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//Class that handles behavior for UI elements on the game's title screen
public class TitleScreenBehavior : MonoBehaviour
{
    //Loads the scene with index 1 within the game's build settings
    //Used within the StartGame button element
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    //Exits the application
    //Used within the ExitGame button element
    public void ExitGame()
    {
        Application.Quit();
    }
}
