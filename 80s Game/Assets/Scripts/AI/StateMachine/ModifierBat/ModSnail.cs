using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class ModSnail : AbsModifierEffect
{
    ModType thisType = ModType.Snail;
    /// <summary>
    /// Override: Activates slow movement effect
    /// </summary>
    public override void ActivateEffect()
    {
        Destroy(modifierUIRefs[0]);
        PlayerInput[] pIs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        for (int i = 0; i < pIs.Length; i++)
        {
            PlayerInputWrapper piw = pIs[i].GetComponent<PlayerInputWrapper>();
            PlayerController pc = piw.GetPlayer();
            if (pc.Order == activator.Order)
            {
                activator.AddModToCount(thisType);
                continue;
            }
            piw.isSlowed = true;
            AddUIRef(pc.Order);
        }
        GameManager.Instance.isSlowed = true;
        GameManager.Instance.debuffActive = true;
        GameManager.Instance.UIManager.postProcessVolume.profile.GetSetting<Vignette>().active = true;
    }

    /// <summary>
    /// Override: Deactivates slow movement effect
    /// </summary>
    public override void DeactivateEffect()
    {
        bool slowedRemains = false;
        PlayerInput[] pIs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        for (int i = 0; i < pIs.Length; i++)
        {
            PlayerInputWrapper piw = pIs[i].GetComponent<PlayerInputWrapper>();
            if (piw.GetPlayer().Order == activator.Order)
            {
                continue;
            }
            piw.isSlowed = false;

        }

        if (!slowedRemains)
        {
            GameManager.Instance.isSlowed = false;
            GameManager.Instance.debuffActive = false;
        }
        GameManager.Instance.UIManager.postProcessVolume.profile.GetSetting<Vignette>().active = false;
    }
    
}
