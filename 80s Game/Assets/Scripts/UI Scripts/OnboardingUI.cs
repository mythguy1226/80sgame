using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingUI : MonoBehaviour
{
    public GameObject onboardingPanel;

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
        Time.timeScale = 1.0f;
    }
}
