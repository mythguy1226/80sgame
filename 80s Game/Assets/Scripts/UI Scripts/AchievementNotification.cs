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
