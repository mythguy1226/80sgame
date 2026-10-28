using Steamworks;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Represents the player avatar in the campaign map
/// </summary>
public class PlayerAvatar : MonoBehaviour
{
    public static Action submitReceived;

    private void HandleNavigate(MapDirection direction)
    {
        if(direction == MapDirection.NONE)
        {
            throw new System.ArgumentException("Direction passed into navigation should never be none");
        }
    }


    private void OnSubmit(InputValue inputValue)
    {
        Debug.Log("submitReceived");
        submitReceived?.Invoke();

    }




    private void OnNavigate(InputValue inputValue)
    {
        Vector2 navigationInput = inputValue.Get<Vector2>();
        bool isZero = navigationInput == Vector2.zero;
        if (!isZero)
        {
            Debug.Log(navigationInput);
            HandleNavigate(NavigationToDirection(navigationInput));
        }
    }

    private MapDirection NavigationToDirection(Vector2 navigation)
    {

        if (navigation.y > 0)
        {
            return MapDirection.UP;
        }
        if (navigation.y < 0)
        {
            return MapDirection.DOWN;
        }
        if(navigation.x < 0)
        {
            return MapDirection.LEFT;
        }
        if (navigation.x > 0)
        {
            return MapDirection.RIGHT;
        }
        return MapDirection.NONE;
    }
}