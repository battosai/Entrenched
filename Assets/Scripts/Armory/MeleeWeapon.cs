
using UnityEngine;
      
[CreateAssetMenu(menuName = ("Wargear/Melee"))]
public class MeleeWeapon : ScriptableObject
{
    [Header("Stats")]
    public float range;
    //TODO:
    //implement speed limitations, aka cooldowns
    //NEED: do it
    public float speed;
    //TODO:
    //decide what to do with ap, maybe cleave level?
    //NEED: design work
    public int ap;
    public int dmg;

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