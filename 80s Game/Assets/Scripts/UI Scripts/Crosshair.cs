using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public SpriteRenderer sprite;
    Vector2 movementDelta;

    public Image overheatUI;

    //Cursor clamping
    float minY, maxY, minX, maxX;
    bool _bIsJiggling;
    float jiggleWidth = 0.005f;
    float jiggleHeight = 0.003f;
    float jiggleFrequency = 50.0f;
    float jigglePhaseShift = 2.0f;
    float currentJiggleTimer;

    Color startColor;
    Color endColor;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        SetClamps();
        //Hide mouse cursor
        Cursor.visible = false;
        currentJiggleTimer = 0.0f;
        _bIsJiggling = false;
        startColor = overheatUI.color;
        endColor = new Color(1f, 0.5f, 0f);
    }

    void Update()
    {
        float jiggleOffsetX = 0.0f;
        float jiggleOffsetY = 0.0f;
        if (_bIsJiggling)
        {
            currentJiggleTimer += Time.deltaTime;
            jiggleOffsetX = Mathf.Sin(jiggleFrequency * currentJiggleTimer) * jiggleWidth;
            jiggleOffsetY = Mathf.Sin(jiggleFrequency * currentJiggleTimer + jigglePhaseShift) * jiggleHeight;
            overheatUI.color = Color.Lerp(startColor, endColor, Mathf.Sin(jiggleFrequency * currentJiggleTimer * 0.5f) * 0.5f + 0.5f);
        }

        float finalX = transform.position.x + movementDelta.x + jiggleOffsetX;
        float finalY = transform.position.y + movementDelta.y + jiggleOffsetY;
        Vector3 newPosition = Clamp(new Vector3(finalX, finalY, transform.position.z));
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

    public void Center()
    {
        transform.position = Vector3.zero;
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

    public void ChangeSpriteColor(Color col)
    {
        sprite.color = col;
    }

    public Sprite GetCrosshairSprite()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        return sprite.sprite;
    }

    public void SetCrosshairSprite(Sprite newSprite)
    {
        sprite.sprite = newSprite;
    }

    public void EnableVentEffects()
    {
        _bIsJiggling = true;
    }

    public void StopVentEffects()
    {
        _bIsJiggling = false;
        currentJiggleTimer = 0;
        overheatUI.color = startColor;
    }
}