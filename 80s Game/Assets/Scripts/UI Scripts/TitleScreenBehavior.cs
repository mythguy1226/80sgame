using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//Class that handles behavior for UI elements on the game's title screen
public class TitleScreenBehavior : MonoBehaviour
{
    public GameObject onboardingContinueButton;
    public GameObject onboardingPanel;

    void Update()
    {
        //Disable button to close onboarding
        if (GameManager.Instance.InputManager.joycons.Count == 0)
        {
            onboardingContinueButton.SetActive(false);
        }

        //only allow user to continue after they recenter the cursor
        else
        {
            Joycon j = GameManager.Instance.InputManager.joycons[GameManager.Instance.InputManager.jc_ind];
            if (j.GetButtonDown(Joycon.Button.DPAD_DOWN))
            {
                onboardingContinueButton.SetActive(true);
            }
        }
    }

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

    //Closes the onboarding panel
    public void CloseOnboarding()
    {
        onboardingPanel.SetActive(false);
    }
}
