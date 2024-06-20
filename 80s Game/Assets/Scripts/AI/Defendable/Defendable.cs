using System.Collections.Generic;
using UnityEngine;

public class Defendable : MonoBehaviour
{
    // The class that governs a defendable object's behavior
    // This object has a several child transforms object whose transform are used as latch points for targets
    // It also has a particle system and an array of sprites that help give feedback to the player
    // on levels of damage
    // Lastly, an optional field for a health bar is included
    [SerializeField]
    int _maxHitpoints;

    [Tooltip("Hp Thresholds to switch to the next sprite. Add these in decreasing order.")]
    [SerializeField]
    List<float> hpThresholds;
    
    [Tooltip("Sprites to use for different levels of damage")]
    [SerializeField]
    List<Sprite> damageLevelSprites;

    [Tooltip("The hp-threshold index from which the particle system should start spawning smoke")]
    [SerializeField]
    int _smokeIndex;

    [Tooltip("Particles for showing a smoke effect at a certain HP threshold")]
    [SerializeField]
    ParticleSystem smokeSystem;

    int _currentHitpoints;
    int _thresholdIndex;

    List<LatchPoint> latchPoints;
    SpriteRenderer sr;
    HealthBar healthbar;


    [HideInInspector]
    public bool bCanBeTargeted;

    private void Start()
    {
        bCanBeTargeted = true;
        sr = GetComponent<SpriteRenderer>();
        smokeSystem.Stop();
        latchPoints = new List<LatchPoint>();
        foreach (Transform child in transform)
        {
            latchPoints.Add(child.GetComponent<LatchPoint>());
        }
        _thresholdIndex = 0;

        _currentHitpoints = _maxHitpoints;
        if (healthbar != null)
        {
            healthbar.Config(_maxHitpoints);
        }
    }

    /// <summary>
    /// Have this defendable object take some damage
    /// </summary>
    /// <param name="damage">The amount of damage to receive</param>
    public void TakeDamage(int damage)
    {
        _currentHitpoints -= damage;
        if (_currentHitpoints <= 0)
        {
            bCanBeTargeted = false;
        }
        if (healthbar != null)
        {
            healthbar.DecreaseValue(damage);
        }
        for(int i = _thresholdIndex + 1;  i < hpThresholds.Count; i++)
        {
            if (_currentHitpoints < hpThresholds[i])
            {
                _thresholdIndex = i;
                UpdateSprite();
            }
        }

        // Start the smoke emission
        if (_thresholdIndex >= _smokeIndex && !smokeSystem.isEmitting)
        {
            smokeSystem.Play();
        }
    }

    /// <summary>
    /// Update the sprite to the threshold index
    /// </summary>
    private void UpdateSprite()
    {
        // Immediately change the sprite to the next 
        sr.sprite = damageLevelSprites[_thresholdIndex];

        // We might want to extend this change with an animation and some sound
    }


    // Bats that collide with the latch radius should be offered a latch point, if one exists
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Filter out non-enemies
        if (!collision.gameObject.CompareTag("Enemy") || !bCanBeTargeted)
        {
            return;
        }

        List<LatchPoint> openLatches = new List<LatchPoint>();
        // Get a random latch point
        foreach(LatchPoint latch in latchPoints)
        {
            if (latch.isAvailable)
            {
                openLatches.Add(latch);
            }
        }


        // No open latches available
        if (openLatches.Count == 0)
        {
            return;
        }

        int offerLatch = Random.Range(0, openLatches.Count);
        openLatches[offerLatch].LatchTarget(collision.gameObject.GetComponent<Target>());
    }
}