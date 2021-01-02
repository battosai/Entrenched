using UnityEngine;

public abstract class Weapon : ScriptableObject
{
    [Header("Stats")]
    public float range;
    public int ap;
    public int dmg;
}