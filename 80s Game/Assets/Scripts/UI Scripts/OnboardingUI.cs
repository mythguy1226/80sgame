using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnboardingUI : MonoBehaviour
{
    //Fields for object references
    public GameObject onboardingPanel;
    public GameObject gameUIElements;
    public TMP_Text onboardingText;
    public TMP_Text pauseControls;
    
    //Fields for onboarding text to be displayed to the player
    private string mouseInputText = "The bat bots have escaped containment! \n\nAim your stun gun with the mouse and fire with left-click to stun the bat bots.";
    private string joyconInputText = "The bat bots have escaped containment! \n\nAim your stun gun by tilting the JoyCon. \n\nFire with the trigger (ZR/ZL) to stun the bat bots.";

    private string joyconPauseControls = "(+/-) Pause";
    private string mousePauseControls = "(ESC) Pause";

    // Start is called before the first frame update
    void Start()
    {
        //Pause game when onboarding panel is activated
        Time.timeScale = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Set the text appropriately depending on if the player is using mouse or joycon input
        onboardingText.SetText(mouseInputText);
        pauseControls.SetText(mousePauseControls);
    }

    //Close the panel, active game UI elements, and unpause the game
    public void CloseOnboarding()
    {
        onboardingPanel.SetActive(false);
        gameUIElements.SetActive(true);

        Time.timeScale = 1.0f;
    }
}
