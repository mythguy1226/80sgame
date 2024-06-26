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
        StartCoroutine(AllowPursue());
    }

    /// <summary>
    /// Coroutine used for delaying time of pursuit
    /// </summary>
    IEnumerator AllowPursue()
    {
        yield return new WaitForSeconds(2.0f);
        bCanPursue = true;
    }
}
