using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AchievementNotification : MonoBehaviour
{
    public void FinishAnimation()
    {
        GameManager.Instance.UIManager.ClearNotification();
        Destroy(this.gameObject);
    }
}

public struct AchievementNotificationData
{
    public string text;
    public Sprite image;

    public AchievementNotificationData(string t, Sprite i)
    {
        text = t;
        image = i;
    }
}