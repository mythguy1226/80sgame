using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitlePlayer : MonoBehaviour
{
    [SerializeField]
    CutsceneManager cutsceneManager;
    string currentControlScheme;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnControlsChanged(PlayerInput newInput)
    {
        //Debug.Log("Controls Changed");
        Debug.Log(newInput.currentControlScheme);
        if ( newInput.currentControlScheme == currentControlScheme || PlayerData.activePlayers.Count == 0)
        {
            return;
        }

        // Inform the fucking press
        PlayerConfig config = PlayerData.activePlayers[0];
        config.device = newInput.devices[0];
        config.controlScheme = newInput.currentControlScheme;
        SettingsManager.Instance.ToggleUIButtons(0);
        cutsceneManager.UpdatePrompts();
    }

    private void OnStartGame()
    {
        cutsceneManager.SkipCutscene();
    }
}
