using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    // Reference to lightning particle system
    ParticleSystem lightningEffect;

    // Start is called before the first frame update
    void Awake()
    {
        lightningEffect = GetComponentInChildren<ParticleSystem>();
    }

    // Method for playing the chain effect
    public void PlayEffect(Vector3 start, Vector3 end)
    {
        // Play effect
        lightningEffect.Play();

        // Init emission params to override positional values
        var emitParams = new ParticleSystem.EmitParams();

        // Start pos
        emitParams.position = start;
        lightningEffect.Emit(emitParams, 1);

        // Middle pos
        emitParams.position = (start + end) / 2.0f;
        lightningEffect.Emit(emitParams, 1);

        // End pos
        emitParams.position = end;
        lightningEffect.Emit(emitParams, 1);

        // Object cleanup
        Destroy(gameObject, 1.0f);
    }
}
