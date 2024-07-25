using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementNotification : MonoBehaviour
{
    public void FinishAnimation()
    {
        GameManager.Instance.UIManager.ClearNotification();
    }
}
