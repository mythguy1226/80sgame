using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonJoyconHandler : MonoBehaviour
{
    private Button _button;
    void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void ActivateButton()
    {
        if (GameManager.Instance.InputManager.MouseLeftDownThisFrame)
        {
            _button.onClick.Invoke();
        }
    }
}
