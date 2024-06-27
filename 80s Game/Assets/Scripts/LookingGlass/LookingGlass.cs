using UnityEngine;

public class LookingGlass : MonoBehaviour
{

    /*
     Bat Controls
    */

    /// <summary>
    /// Tells the target manager to spawn a bat of the specific type
    /// </summary>
    /// <param name="type">The type of bat to spawn</param>
    public void SpawnBat(int type)
    {
        GameManager.Instance.TargetManager.SpawnTarget((TargetManager.TargetType)type);
    }


    /// <summary>
    /// Tells the active game mode to toggle whether a certain type of bat can spawn or not
    /// </summary>
    /// <param name="type"></param>
    public void ToggleCanSpawn(int type)
    {
        TargetManager.TargetType toggleType = (TargetManager.TargetType)type;
        Debug.Log(toggleType);
        GameManager.Instance.ActiveGameMode.ToggleAllowedBatType(toggleType);
    }
}