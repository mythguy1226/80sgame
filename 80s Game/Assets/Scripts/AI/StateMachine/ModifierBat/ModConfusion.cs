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
            PlayerInputWrapper piw = pIs[i].GetComponent<PlayerInputWrapper>();
            PlayerController pc = piw.GetPlayer();
            if (pc.Order == activator.Order)
            {
                activator.AddModToCount(GetModType());
                continue;
            }
            piw.isFlipped = true;
            AddUIRef(pc.Order);

        }
        GameManager.Instance.debuffActive = true;
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
        GameManager.Instance.debuffActive = false;
    }
}
