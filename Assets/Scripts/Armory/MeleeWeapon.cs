
using UnityEngine;
      
[CreateAssetMenu(menuName = ("Wargear/Melee"))]
public class MeleeWeapon : Weapon
{
    public float cooldown;

    [Header("Sounds")]
    public AudioClip[] swings;
    public AudioClip[] hits;

    [Header("Animations")]
    public AnimationClip standIdle;
    public AnimationClip crouchIdle;
    public AnimationClip run;
    public AnimationClip standAttack;
    public AnimationClip crouchAttack;
    public AnimationClip standEquip;
    public AnimationClip crouchEquip;
    public AnimationClip standUnequip;
    public AnimationClip crouchUnequip;
}