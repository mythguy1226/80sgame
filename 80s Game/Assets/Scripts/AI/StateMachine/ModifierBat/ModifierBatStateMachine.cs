using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ModifierBatStateMachine : BatStateMachine
{
    // Public modifier fields
    bool modifierDropped = false;

    /// <summary>
    /// Override: Start method
    /// </summary>
    void Start()
    {
    }

    /// <summary>
    /// Override: Checks if target has been stunned
    /// </summary>
    public override void ResolveHit()
    {
        // Trigger base behavior
        base.ResolveHit();

        // Instantiate the modifier object
        if (!modifierDropped)
        {
            List<GameObject> buffs = GameManager.Instance.GetBuffs();
            List<GameObject> debuffs = GameManager.Instance.GetDebuffs();
            Debug.Log("Debuffs: " + debuffs.Count.ToString());
            //Compose available modifiers into single list
            List<GameObject> modifierObjects = new List<GameObject>();
            foreach(GameObject buff in buffs)
            {
                modifierObjects.Add(buff);
            }
            if (!GameManager.Instance.debuffActive || GameManager.Instance.ActiveGameMode.isInDebugMode())
            {
                foreach(GameObject debuff in debuffs)
                {
                    modifierObjects.Add(debuff);
                }
            }

            // Guard Clause
            if (modifierObjects.Count == 0)
            {
                return;
            }

            // Alter this if you want to force a specific modifier to drop
            int randomIndex = Random.Range(0, modifierObjects.Count);
            Instantiate(modifierObjects[randomIndex], transform.position, Quaternion.identity);
            modifierDropped = true;
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
