using UnityEngine;
      
[CreateAssetMenu(menuName = ("Wargear/Ranged"))]
public class RangedWeapon : ScriptableObject
{
    [Header("Stats")]
    public int ammo;
    public float chargeTime;
    public int ap;
    public int dmg;

    [Header("Animations")]
    public AnimationClip standIdle;
    public AnimationClip crouchIdle;
    public AnimationClip run;
    public AnimationClip standShoot;
    public AnimationClip crouchShoot;
    public AnimationClip standReload;
    public AnimationClip crouchReload;
    public AnimationClip standEquip;
    public AnimationClip crouchEquip;
    public AnimationClip standUnequip;
    public AnimationClip crouchUnequip;
}