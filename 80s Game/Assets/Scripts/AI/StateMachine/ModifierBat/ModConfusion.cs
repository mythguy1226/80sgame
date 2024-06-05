using UnityEngine;
using UnityEngine.InputSystem;

public class ModConfusion : AbsModifierEffect
{
    /// <summary>
    /// Override: Activates inverse controller effect
    /// </summary>
    public override void ActivateEffect()
    {
        PlayerInput[] pIs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        for(int i = 0; i < pIs.Length; i++)
        {
            PlayerInputWrapper piw = pIs[i].GetComponent<PlayerInputWrapper>();
            if (piw.GetPlayer() == activator)
            {
                continue;
            }
            piw.isFlipped = true;
        }
    }

    /// <summary>
    /// Override: Deactivates inverse controller effect
    /// </summary>
    public override void DeactivateEffect()
    {
        PlayerInput[] pIs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        for (int i = 0; i < pIs.Length; i++)
        {
            PlayerInputWrapper piw = pIs[i].GetComponent<PlayerInputWrapper>();
            if (piw.GetPlayer() == activator)
            {
                continue;
            }
            piw.isFlipped = false;
        }
    }
}
