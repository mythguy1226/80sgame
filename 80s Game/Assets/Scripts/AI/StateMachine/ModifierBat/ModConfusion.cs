using UnityEngine;
using UnityEngine.InputSystem;

public class ModConfusion : AbsModifierEffect
{
    public override ModType GetModType()
    {
        return ModType.Confusion;
    }
    /// <summary>
    /// Override: Activates inverse controller effect
    /// </summary>
    public override void ActivateEffect()
    {
        Destroy(modifierUIRefs[0]);
        PlayerInput[] pIs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        for(int i = 0; i < pIs.Length; i++)
        {
            // Get player input wrapper and its controller
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
                piw.isFlipped = true;
                AddUIRef(pc.Order);
                pc.SetMod(GetModType(), this);
                continue;
            }
            else if (pc.Order != activator.Order && !bIsSelfDebuff) // Affect all players but activator
            {
                piw.isFlipped = true;
                AddUIRef(pc.Order);
                pc.SetMod(GetModType(), this);
                continue;
            }
            else if(pc.Order == activator.Order && bIsSelfDebuff) // Affect the activator
            {
                piw.isFlipped = true;
                AddUIRef(pc.Order);
                pc.SetMod(GetModType(), this);
                continue;
            }
            activator.AddModToCount(GetModType());

        }
        GameManager.Instance.debuffActive = true;
        HandleModifierCountAchievement();
    }

    /// <summary>
    /// Override: Deactivates inverse controller effect
    /// </summary>
    public override void DeactivateEffect()
    {
        PlayerInput[] pIs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        for (int i = 0; i < pIs.Length; i++)
        {
            // Get player input wrapper
            PlayerInputWrapper piw = pIs[i].GetComponent<PlayerInputWrapper>();
            if (piw.GetPlayer() == activator && !bIsSelfDebuff) // Continue if disabling other players
            {
                continue;
            }
            else if (piw.GetPlayer() == activator && bIsSelfDebuff) // Continue if disabling activator
            {
                piw.isFlipped = false;
                piw.GetPlayer().RemoveMod(GetModType());
                continue;
            }
            piw.isFlipped = false;
            piw.GetPlayer().RemoveMod(GetModType());
            AchievementManager.RegisterData("confmod-pi-" + i.ToString(), 0);
        }
        GameManager.Instance.debuffActive = false;
    }
}
