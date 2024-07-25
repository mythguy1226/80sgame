using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

public class ModSnail : AbsModifierEffect
{
    public override ModType GetModType()
    {
        return ModType.Snail;
    }
    /// <summary>
    /// Override: Activates slow movement effect
    /// </summary>
    public override void ActivateEffect()
    {
        PlayerInput[] pIs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        for (int i = 0; i < pIs.Length; i++)
        {
            PlayerInputWrapper piw = pIs[i].GetComponent<PlayerInputWrapper>();
            PlayerController pc = piw.GetPlayer();

            // If player already has mod extend it
            if (pc.HasMod(GetModType()))
            {
                pc.ExtendModDuration(GetModType(), maxEffectDuration);
                CleanUp();
                continue;
            }
            else if (GameManager.Instance.gameModeType == EGameMode.Defense) // Affect all players in defense mode
            {
                Debug.Log("Defense Mode!");
                piw.isSlowed = true;
                AddUIRef(pc.Order);
                pc.SetMod(GetModType(), this);
                continue;
            }
            else if (pc.Order != activator.Order && !bIsSelfDebuff) // Affect all players but activator
            {
                piw.isSlowed = true;
                AddUIRef(pc.Order);
                pc.SetMod(GetModType(), this);
                continue;
            }
            else if(pc.Order == activator.Order && bIsSelfDebuff) // Affect the activator
            {
                piw.isSlowed = true;
                AddUIRef(pc.Order);
                pc.SetMod(GetModType(), this);
                continue;
            }
            activator.AddModToCount(GetModType());
        }
        GameManager.Instance.isSlowed = true;
        GameManager.Instance.debuffActive = true;
        GameManager.Instance.UIManager.postProcessVolume.profile.GetSetting<Vignette>().active = true;
        HandleModifierCountAchievement();
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
            if (piw.GetPlayer() == activator && !bIsSelfDebuff) // Continue if disabling other players
            {
                continue;
            }
            else if (piw.GetPlayer() == activator && bIsSelfDebuff) // Continue if disabling activator
            {
                piw.isSlowed = false;
                piw.GetPlayer().RemoveMod(GetModType());
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
