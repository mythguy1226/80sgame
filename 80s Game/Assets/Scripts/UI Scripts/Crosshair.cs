using UnityEditor;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    Sprite sprite;
    Vector2 movementDelta;

    //Cursor clamping
    float minY, maxY, minX, maxX;

    private void Awake()
    {
        SetClamps();
        //Hide mouse cursor
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 newPosition = Clamp(new Vector3(transform.position.x + movementDelta.x, transform.position.y + movementDelta.y, transform.position.z));
        transform.position = newPosition;
    }

    public void SetMovementDelta(Vector2 movementData)
    {
        movementDelta = movementData;
    }

    public Vector3 PositionToScreen()
    {
        Camera cam = Camera.main;
        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        return screenPos;
    }

    private Vector3 Clamp(Vector3 position)
    {
        Vector3 adjustedPosition = position;
        if (position.x < minX)
        {
            adjustedPosition.x = minX;
        }

        if (position.x > maxX)
        {
            adjustedPosition.x = maxX;
        }

        if (position.y < minY)
        {
            adjustedPosition.y = minY;
        }

        if (position.y > maxY)
        {
            adjustedPosition.y = maxY;
        }
        return adjustedPosition;
    }

    private void SetClamps()
    {
        Camera cam = Camera.main;
        maxY = cam.ScreenToWorldPoint(new Vector3(0, Screen.height, cam.transform.position.z)).y;
        minY = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.transform.position.z)).y;
        minX = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.transform.position.z)).x;
        maxX = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cam.transform.position.z)).x;
    }
}