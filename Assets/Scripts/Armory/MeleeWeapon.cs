
using UnityEngine;
      
[CreateAssetMenu(menuName = ("Wargear/Melee"))]
public class MeleeWeapon : ScriptableObject
{
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

    [Header("Stats")]
    public float speed;
    public int ap;
    public int dmg;
}