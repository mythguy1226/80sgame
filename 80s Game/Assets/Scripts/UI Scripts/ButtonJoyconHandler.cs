using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonJoyconHandler : MonoBehaviour
{
    //field for if the button is the pause button
    public bool pauseButton = false;

    private Button _button;
    private bool hovered;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Update()
    {
        //If the joycon trigger is pressed when the button is hovered over, perform the button's action
        if (GameManager.Instance.InputManager.MouseLeftDownThisFrame && hovered)
        {
            _button.onClick.Invoke();
            hovered = false;

            if (!pauseButton)
            {
                Invoke("ButtonHover", 0.1f);
            }    
        }
    }

    //Methods used for registering if the button is hovered over or not
    //Attach to the PointerEnter and PointerExit event triggers on the button's game object
    public void ButtonHover()
    {
        hovered = true;
    }

    public void ButtonUnhover()
    {
        hovered = false;
    }
}
