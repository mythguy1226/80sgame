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
    public int _maxHitpoints;

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
    [SerializeField]
    ParticleSystem forceFieldParticles;
    [SerializeField]
    ParticleSystem burstParticles;

    [SerializeField]
    AudioClip damageSFX;

    public int _currentHitpoints;
    int _thresholdIndex;

    public List<LatchPoint> latchPoints;
    SpriteRenderer sr;
    Animator animator;
    HealthBar healthbar;

    [HideInInspector]
    public bool bCanBeTargeted;

    public bool bIsCore = false;

    [Tooltip("Object responsible for local positioning of sprite animation")]
    [SerializeField]
    private GameObject spriteObject;

    private void Start()
    {
        bCanBeTargeted = true;
        sr = spriteObject.GetComponent<SpriteRenderer>();
        animator = spriteObject.GetComponent<Animator>();
        smokeSystem.Stop();
        latchPoints = new List<LatchPoint>();
        foreach (Transform child in transform)
        {
            // SpriteObject is now also a child
            if(child.gameObject.name == "LatchPoint")
            {
                latchPoints.Add(child.GetComponent<LatchPoint>());
            }
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
        animator.SetTrigger("TakeDamage");
        SoundManager.Instance.PlaySoundInterrupt(damageSFX, bIsCore ? 0.5f : .9f, bIsCore ? .7f : 1.1f);

        _currentHitpoints -= damage;
        if (_currentHitpoints <= 0)
        {
            bCanBeTargeted = false;
            foreach(LatchPoint latch in latchPoints)
            {
                latch.Unlatch();
            }

            sr.color = Color.red;
            animator.SetBool("IsDead", true);

            UpdateForceField();
        }
        if (healthbar != null)
        {
            healthbar.DecreaseValue(damage);
        }

        for(int i = _thresholdIndex; i < hpThresholds.Count; i++)
        {
            if(_currentHitpoints < hpThresholds[i])
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
        sr.sprite = damageLevelSprites[_thresholdIndex + 1];

        // We might want to extend this change with an animation and some sound
    }


    private void UpdateForceField()
    {
        forceFieldParticles.emission.SetBurst(0,
            new ParticleSystem.Burst(
                0,
                new ParticleSystem.MinMaxCurve(
                    forceFieldParticles.emission.GetBurst(0).count.constant / 4
                ),
                0,
                0.01f
            )
        );

        burstParticles.Play();
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