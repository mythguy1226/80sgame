using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Public fields
    public float projectileSpeed;
    public GameObject impactParticles;
    public float lifeTime = 3.0f;

    // Update is called once per frame
    void Update()
    {
        // Update timer for life-time and destroy object once it ends
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0.0f)
            Destroy(gameObject);
    }

    // Method used for checking overlap collisions
    void OnCollisionEnter2D(Collision2D other)
    {
        // If the two game objects share the same tag
        // then ignore the collision and return
        if (gameObject.tag == other.gameObject.tag)
            return;

        // Try getting a target component from other collider
        Target hitTarget = other.gameObject.GetComponent<Target>();
        if(hitTarget != null) // null check
            hitTarget.ResolveHit();

        // Destroy this object
        Destroy(gameObject);

        // Spawn impact particles and destroy on timer
        GameObject impact = Instantiate(impactParticles, transform.position, transform.rotation);
        Destroy(impact, 1.0f);
    }
}
