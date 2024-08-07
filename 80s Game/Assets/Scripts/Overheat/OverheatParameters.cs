using UnityEngine;

[CreateAssetMenu(menuName ="ScriptableObjects/OverheatParameters")]
public class OverheatParameters : ScriptableObject
{
    public float overheatMax;
    public float maxShootOverheat;
    public float heatAdd;
    public float heatRemove;
}