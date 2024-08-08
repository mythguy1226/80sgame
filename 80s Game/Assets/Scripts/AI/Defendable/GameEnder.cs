using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnder : MonoBehaviour
{
    private void EndGame()
    {
        // Cast active game mode to coop mode and call custom end method
        CooperativeMode coopMode = (CooperativeMode)GameManager.Instance.ActiveGameMode;
        if (coopMode != null)
            coopMode.EndCoopGame();
    }
}
