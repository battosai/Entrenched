
using UnityEngine;
      
[CreateAssetMenu(menuName = ("Wargear/Melee"))]
public class MeleeWeapon : Weapon
{
    //TODO:
    //implement speed limitations, aka cooldowns
    //NEED: do it, maybe scrap speed and implement a durability to melee?
    public float speed;

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