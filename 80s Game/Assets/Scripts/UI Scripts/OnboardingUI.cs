using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingUI : MonoBehaviour
{
    public GameObject onboardingPanel;
    public GameObject gameUIElements;

    // Start is called before the first frame update
    void Start()
    {
        //Pause game when onboarding panel is activated
        Time.timeScale = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Close the panel, active game UI elements, and unpause the game
    public void CloseOnboarding()
    {
        onboardingPanel.SetActive(false);
        gameUIElements.SetActive(true);

        Time.timeScale = 1.0f;
    }
}
