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
        if (GameManager.Instance.InputManager.MouseLeftDownThisFrame && hovered)
        {
            _button.onClick.Invoke();
            hovered = false;
        }
    }

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
