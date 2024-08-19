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
                AnimateUIRef();
                pc.SetMod(GetModType(), this);
                continue;
            }
            else if (pc != activator && !bIsSelfDebuff) // Affect all players but activator
            {
                piw.isFlipped = true;
                AnimateUIRef();
                pc.SetMod(GetModType(), this);
                continue;
            }
            else if(pc == activator && bIsSelfDebuff) // Affect the activator
            {
                piw.isFlipped = true;
                AnimateUIRef();
                pc.SetMod(GetModType(), this);
                continue;
            }
            activator.AddModToCount(GetModType());

        }
        GameManager.Instance.debuffActive = true;
        HandleModifierCountAchievement();
    }

    // Called once every frame
    void Update()
    {
        // Call base method
        base.Update();

        // Make all players affected rotate their cursors while
        // modifier is active
        if(bIsActive)
        {
            // Iterate through each player
            PlayerInput[] pIs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
            for(int i = 0; i < pIs.Length; i++)
            {
                // Get player input wrapper and its controller
                PlayerInputWrapper piw = pIs[i].GetComponent<PlayerInputWrapper>();
                PlayerController pc = piw.GetPlayer();

                // If player has mod, rotate their cursor
                if (pc.HasMod(GetModType()))
                {
                    // Get player's crosshair
                    Crosshair ch = pc.activeCrosshair;

                    // Update rotation of crosshair
                    ch.transform.rotation *= Quaternion.AngleAxis(0.2f, new Vector3(0.0f, 0.0f, 1.0f));
                }
            }
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
                piw.GetPlayer().activeCrosshair.transform.rotation = Quaternion.identity;
                continue;
            }
            piw.isFlipped = false;
            piw.GetPlayer().RemoveMod(GetModType());
            piw.GetPlayer().activeCrosshair.transform.rotation = Quaternion.identity;
            AchievementManager.RegisterData("confmod-pi-" + i.ToString(), 0);
        }
        GameManager.Instance.debuffActive = false;
    }
}
