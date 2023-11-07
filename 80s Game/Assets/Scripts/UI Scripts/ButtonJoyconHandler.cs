using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonJoyconHandler : MonoBehaviour
{
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
        }
    }

    //Methods used for registering if the button is hovered over or not
    //Attach to the PointerEnter and PointerExit event triggers on the button's game object
    public void ButtonHover()
    {
        hovered = true;
        Debug.Log("Mouse Entered");
    }

    public void ButtonUnhover()
    {
        hovered = false;
        Debug.Log("Mouse Exited");
    }
}
