using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsGameMode
{
    public enum EGameMode
    {
        Classic,
        Competitive,
        Defense
    }

    public EGameMode modeType;
    public int speedModifier;
    public bool gameOver = false;

    public abstract void OnTargetStun();

    public abstract int GetNextAvailableBat();
}
