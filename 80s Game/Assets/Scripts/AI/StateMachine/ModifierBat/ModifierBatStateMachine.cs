using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierBatStateMachine : BatStateMachine
{
    // Public modifier fields
    public GameObject modifierObject;
    bool modifierDropped = false;

    /// <summary>
    /// Override: Checks if target has been stunned
    /// </summary>
    public override void DetectStun(Vector3 pos)
    {
        // Check time scale so bats cant be harmed while game is paused
        bool isGameGoing = Time.timeScale > 0;
        if (!isGameGoing)
        {
            return;
        }

        // Check for player input coords hitting target
        Vector3 shotPos = pos;
        RaycastHit2D hit = Physics2D.Raycast(shotPos, Vector2.zero);

        // Null check
        if (!hit)
            return;

        // Check that hit has detected this particular object
        if (hit.collider.gameObject == gameObject)
        {
            _AnimControls.PlayStunAnimation();
            SoundManager.Instance.PlaySoundInterrupt(hitSound, 0.7f);

            // Instantiate the modifier object
            if(!modifierDropped)
            {
                Instantiate(modifierObject, transform.position, Quaternion.identity);
                modifierDropped = true;
            }    
        }
    }

    /// <summary>
    /// Override: Resets target
    /// </summary>
    public override void Reset()
    {
        base.Reset();

        // Reset drop flag
        modifierDropped = false;
    }
}
