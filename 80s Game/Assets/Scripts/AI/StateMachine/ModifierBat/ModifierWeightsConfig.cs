using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CLASS: Modifier Weights Config
 * Scriptable Object used for storing
 * all information on modifier spawn chances
 */
[CreateAssetMenu(menuName = "ScriptableObjects/ModifierWeightsConfig")]
public class ModifierWeightsConfig : ScriptableObject
{
    public List<ModifierWeight> weights;
}

// Struct used for storing weight data
[System.Serializable]
public struct ModifierWeight
{
    public AbsModifierEffect.ModType modType;
    public float chance;
}
