using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModRustedWings : AbsModifierEffect
{
    public GameObject particles;
    public List<GameObject> particlesList;

    public override ModType GetModType()
    {
        return ModType.RustedWings;
    }

    public override void ActivateEffect()
    {
        // If mod is already active extend the mod duration
        if (activator.HasMod(GetModType()))
        {
            activator.ExtendModDuration(GetModType(), effectDuration);
            CleanUp();
            return;
        }

        // Update game manager to slow down bats
        affectedPlayers.Add(activator);
        activator.SetMod(GetModType(), this);
        GameManager.Instance.isSlowed = true;
        GameManager.Instance.rustedWingsStack++;
        HandleModifierCountAchievement();

        // Get list of all targets
        List<Target> targets = GameManager.Instance.TargetManager.targets;
        foreach(Target target in targets)
        {
            // Instantiate particle effects and set to proper parent
            GameObject particle = Instantiate(particles, target.transform);
            particlesList.Add(particle);
            particle.transform.SetParent(target.transform);
        }
    }

    public override void DeactivateEffect()
    {
        // Remove mod from activator and undo
        // slow effect on bats
        activator.RemoveMod(GetModType());
        GameManager.Instance.isSlowed = false;
        if(GameManager.Instance.rustedWingsStack > 0)
        {
            GameManager.Instance.rustedWingsStack--;
        }

        // Clean up and destroy all particles
        foreach(GameObject particle in particlesList)
        {
            if(particle != null)
            {
                Destroy(particle);
            }
        }
    }
}
