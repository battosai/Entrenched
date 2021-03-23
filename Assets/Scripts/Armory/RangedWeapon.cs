using UnityEngine;
      
[CreateAssetMenu(menuName = ("Wargear/Ranged"))]
public class RangedWeapon : Weapon
{
    public int clipSize;
    public float chargeTime;

    [Header("Sounds")]
    public AudioClip charge;
    public AudioClip fullCharge;
    public AudioClip[] shots;
    public AudioClip[] dryShots;
    public AudioClip[] reloads;
    public AudioClip[] hits;

    [Header("Ammo Count")]
    public Sprite[] ammoCounter;

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