using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;

public class ModSnail : AbsModifierEffect
{
    // Reference to VFX prefab
    public GameObject vfx;
    private List<GameObject> activeVFX;

    public override ModType GetModType()
    {
        return ModType.Snail;
    }
    /// <summary>
    /// Override: Activates slow movement effect
    /// </summary>
    public override void ActivateEffect()
    {
        // Init active VFX list
        activeVFX = new List<GameObject>();

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
                piw.isSlowed = true;
                AnimateUIRef();
                pc.SetMod(GetModType(), this);
                EnableVFX(pc);
                continue;
            }
            else if (pc.Order != activator.Order && !bIsSelfDebuff) // Affect all players but activator
            {
                piw.isSlowed = true;
                AnimateUIRef();
                pc.SetMod(GetModType(), this);
                EnableVFX(pc);
                continue;
            }
            else if(pc.Order == activator.Order && bIsSelfDebuff) // Affect the activator
            {
                piw.isSlowed = true;
                AnimateUIRef();
                pc.SetMod(GetModType(), this);
                EnableVFX(pc);
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
        DisableVFX();
    }

    /// <summary>
    /// Applies VFX systems onto the player's cursor
    /// upon activation of modifier
    /// </summary>
    public void EnableVFX(PlayerController pc)
    {
        // Get player's crosshair
        Crosshair ch = pc.activeCrosshair;

        // Instantiate VFX and parent the crosshair to it
        GameObject tempVFX = Instantiate(vfx, ch.transform.position, transform.rotation);
        tempVFX.transform.SetParent(ch.transform);

        // Get particle system and change properties based on player
        ParticleSystem vfxSystem = tempVFX.GetComponentInChildren<ParticleSystem>();

        // Change color gradient to match player's crosshair color
        var col = vfxSystem.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys( new GradientColorKey[] { new GradientColorKey(ch.sprite.color, 0.0f), new GradientColorKey(ch.sprite.color, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) } );
        col.color = grad;

        // Get texture sheet animation from particle system
        var ts = vfxSystem.textureSheetAnimation;

        // Get the crosshair sprite and its texture
        Sprite chSprite = ch.sprite.sprite;
        Texture chTexture = chSprite.texture;

        // Calculate the x and y coordinates of the crosshair sprite
        // on the texture sheet grid
        float xProg = chSprite.rect.x / chTexture.width;
        int xLoc = (int)(xProg * ts.numTilesX);
        float yProg = chSprite.rect.y / chTexture.height;
        int yLoc = ts.numTilesY - (int)(yProg * ts.numTilesY) - 1;

        // Calculate and set the start frame
        float maxSheetValue = ts.numTilesX * ts.numTilesY;
        float stepValue = 1f / maxSheetValue;

        ts.startFrame = stepValue * (xLoc + ts.numTilesX * yLoc);

        // Add new VFX to the list of active vfx objects
        activeVFX.Add(tempVFX);
    }

    /// <summary>
    /// Disable VFX of modifier for all
    /// who have them applied
    /// </summary>
    public void DisableVFX()
    {
        // Destroy all active instances of the vfx
        foreach(GameObject effect in activeVFX)
        {
            Destroy(effect.gameObject);
        }
    }
}
