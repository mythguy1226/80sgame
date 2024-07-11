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

    /// <summary>
    /// Override: Wait a few seconds before letting bat pursue
    /// and also add indicators to telegraph pursuit
    /// </summary>
    public override void BeginPursue()
    {
        SpriteRenderer.color = Color.red;

        //plays the telegraph ("charge") animation
        base.AnimControls.PlayChargeAnimation();
        StartCoroutine(AllowPursue());
    }

    /// <summary>
    /// Coroutine used for delaying time of pursuit
    /// </summary>
    IEnumerator AllowPursue()
    {
        yield return new WaitForSeconds(2.0f);
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

        SpriteRenderer.color = new Color(255.0f/255.0f, 96.0f/255.0f, 0);
    }
}
