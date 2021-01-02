using UnityEngine;
      
[CreateAssetMenu(menuName = ("Wargear/Ranged"))]
public class RangedWeapon : Weapon
{
    //TODO:
    //actually implement ammo: weapon ammo count is just how many left in clip, actual total ammo will be on krieger, enemies should probably drop ammo
    //NEED: design work
    public int ammo;
    public float chargeTime;

    [Header("Animations")]
    public AnimationClip standIdle;
    public AnimationClip crouchIdle;
    public AnimationClip run;
    public AnimationClip standShoot;
    public AnimationClip crouchShoot;
    public AnimationClip standEmpty;
    public AnimationClip crouchEmpty;
    public AnimationClip standReload;
    public AnimationClip crouchReload;
    public AnimationClip standEquip;
    public AnimationClip crouchEquip;
    public AnimationClip standUnequip;
    public AnimationClip crouchUnequip;
}