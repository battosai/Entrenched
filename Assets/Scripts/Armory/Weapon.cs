using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    [Header("Unlock Distance Threshold")]
    public float unlockDistance;

    [Header("Stats")]
    public float range;
    public int ap;
    public int dmg;
}