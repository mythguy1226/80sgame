using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* CLASS: Spawn Rate Config
 * Scriptable Object used for storing
 * all information on game mode spawn rates
 */
[CreateAssetMenu(menuName = "ScriptableObjects/SpawnRateConfig")]
public class SpawnRateConfig : ScriptableObject
{
    public List<SpawnRate> rates;
}

// Struct used for storing rate data
[System.Serializable]
public struct SpawnRate
{
    public TargetManager.TargetType targetType;
    public int spawnRate;
}
