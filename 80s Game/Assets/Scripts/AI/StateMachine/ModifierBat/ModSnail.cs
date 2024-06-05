using UnityEngine;
using UnityEngine.InputSystem;

public class ModSnail : AbsModifierEffect
{
    /// <summary>
    /// Override: Activates slow movement effect
    /// </summary>
    public override void ActivateEffect()
    {
        PlayerInput[] pIs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        for (int i = 0; i < pIs.Length; i++)
        {
            PlayerInputWrapper piw = pIs[i].GetComponent<PlayerInputWrapper>();
            if (piw.GetPlayer() == activator)
            {
                continue;
            }
            piw.isSlowed = true;
            GameManager.Instance.isSlowed = true;
        }
    }

    /// <summary>
    /// Override: Deactivates slow movement effect
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
            piw.isSlowed = false;
            GameManager.Instance.isSlowed = false;
        }
    }
}
