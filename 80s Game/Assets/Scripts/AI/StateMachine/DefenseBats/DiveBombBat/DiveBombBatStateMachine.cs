using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiveBombBatStateMachine : DefenseBatStateMachine
{
    // Public fields
    public GameObject hitParticles;

    /// <summary>
    /// Override: Makes bat explode, harming the latched defendable and
    /// destroying the bat in the process
    /// </summary>
    public override void Attack()
    {
        // Get the defendable from the target latch
        Defendable latchedDefendable = targetLatch.transform.parent.GetComponent<Defendable>();

        // Deal damage
        latchedDefendable.TakeDamage(attackDamage);

        // Play particle effects
        GameObject hitVFX = Instantiate(hitParticles, transform.position, Quaternion.identity);
        Destroy(hitVFX, 0.6f);

        // Reset the bat
        Reset();
    }

    protected override void Update()
    {
        base.Update();

        if(currentState.StateKey == DefenseBatStates.Attacking)
        {
            Attack();
        }
    }

    /// <summary>
    /// Override: Wait a few seconds before letting bat pursue
    /// and also add indicators to telegraph pursuit
    /// </summary>
    public override void BeginPursue()
    {
        //tint the bat slightly red to signal an upcoming attack
        SpriteRenderer.color = new Color(255.0f / 255.0f, 185.0f / 255.0f, 185.0f / 255.0f);

        //plays the telegraph ("charge") animation
        base.AnimControls.PlayChargeAnimation();
        StartCoroutine(AllowPursue());
    }

    /// <summary>
    /// Coroutine used for delaying time of pursuit
    /// </summary>
    IEnumerator AllowPursue()
    {
        yield return new WaitForSeconds(1.5f);
        bCanPursue = true;
        base.AnimControls.PlayAttackAnimation();
    }

    /// <summary>
    /// Override: Resets specific values for dive bomb bats
    /// </summary>
    public override void Reset()
    {
        base.Reset();

        //since this bat explodes, need to reset animation after death
        base.AnimControls.ResetAnimation();

        SpriteRenderer.color = new Color(255.0f/255.0f, 255.0f/255.0f, 255.0f / 255.0f);
    }

    public override void ResolveHit()
    {
        base.ResolveHit();
        if (bCanPursue) {
            AchievementManager.UnlockAchievement(AchievementConstants.BOMB_VOYAGE);
        }
    }
}
