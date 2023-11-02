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
        }
    }

    public void ButtonHover()
    {
        hovered = true;
    }

    public void ButtonUnhover()
    {
        hovered = false;
    }
}
