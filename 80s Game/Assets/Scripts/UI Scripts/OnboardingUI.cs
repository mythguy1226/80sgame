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
        Time.timeScale = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseOnboarding()
    {
        onboardingPanel.SetActive(false);
        gameUIElements.SetActive(true);

        Time.timeScale = 1.0f;
    }
}
