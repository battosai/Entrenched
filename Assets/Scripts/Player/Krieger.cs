using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Krieger : MonoBehaviour
{
    //player settings
    public float moveSpeed;
    public int ammo;

    //player input
    public bool isMoving;
    public bool isCrouching;
    public bool isAttacking;
    public bool isReloading;
    public bool isSwitchingWeapons;
    public bool isMelee;

    //krieger components
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer torsoRend;
    private SpriteRenderer legsRend;

    //weapon components
    public static string startingRangedWeapon; //thinking this should be read in from a pre-game scene
    public static string startingMeleeWeapon;  //thinking this should be read in from a pre-game scene
    public Armory armory;
    private Transform weaponTrans;
    private Animator weaponAnim;
    private AnimatorOverrideController weaponAnimOverCont;
    private AnimationClipOverrides weaponAnimOverrides;
    private SpriteRenderer weaponRend;
    private RangedWeapon _rangedWeapon;
    public RangedWeapon rangedWeapon
    {
        get
        {
            return this._rangedWeapon;
        }
        private set
        {
            _rangedWeapon = value;
            ammo = _rangedWeapon.ammo;
            UpdateEquippedWeaponAnims();
        }
    }
    private MeleeWeapon _meleeWeapon;
    public MeleeWeapon meleeWeapon
    {
        get
        {
            return this._meleeWeapon;
        }
        private set
        {
            _meleeWeapon = value;
            UpdateEquippedWeaponAnims();
        }
    }

    private void Awake() 
    {
        Debug.Assert(armory != null);
        armory.Define();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        torsoRend = transform.Find("Torso").GetComponent<SpriteRenderer>();
        legsRend = transform.Find("Legs").GetComponent<SpriteRenderer>();

        weaponTrans = transform.Find("Weapon");
        weaponAnim = weaponTrans.GetComponent<Animator>();
        weaponRend = weaponTrans.GetComponent<SpriteRenderer>();
    }

    private void Start() 
    {
        //anim
        weaponAnimOverCont = 
            new AnimatorOverrideController(weaponAnim.runtimeAnimatorController);
        weaponAnim.runtimeAnimatorController = weaponAnimOverCont;
        weaponAnimOverrides = 
            new AnimationClipOverrides(weaponAnimOverCont.overridesCount);
        weaponAnimOverCont.GetOverrides(weaponAnimOverrides);

        //status
        isMelee = false;

        //equip starting weapons
        rangedWeapon = String.IsNullOrEmpty(startingRangedWeapon) ?
            armory.ranged["Lasgun"] :
            armory.ranged[startingRangedWeapon];
        meleeWeapon = String.IsNullOrEmpty(startingMeleeWeapon) ?
            armory.melee["Shovel"] :
            armory.melee[startingMeleeWeapon];
    }

    private void Update()
    {
        ReadInput();
        InputHandler();
    }

    /// <summary>
    /// Reads keyboard input to manipulate status flags.
    /// </summary>
    private void ReadInput()
    {
        //hold downs
        isMoving = false;
        isCrouching = false;
        isAttacking = false;
        if(!isReloading && !isSwitchingWeapons)
            if(Input.GetKey(KeyCode.D))    
                isMoving = true;
        if(!isAttacking)
            if(Input.GetKey(KeyCode.LeftShift))
            {
                isCrouching = true;
                isMoving = false;
            }

        //one taps (and take animation time to reset)
        if(!isReloading && !isSwitchingWeapons)
        {
            if(Input.GetMouseButtonDown(1))
            {
                isSwitchingWeapons = true;
                isMoving = false;
                anim.SetTrigger("SwitchWeapon");
                weaponAnim.SetTrigger("SwitchWeapon");
            }
            if(!isSwitchingWeapons)
            {
                if(Input.GetKeyDown(KeyCode.R))
                {
                    if(!isMelee)
                    {
                        isReloading = true;
                        anim.SetTrigger("Reload");
                        weaponAnim.SetTrigger("Reload");
                    }
                }
            }
        }

        //instant one-ticks
        if(!isReloading && !isSwitchingWeapons)
        {
            if(Input.GetMouseButtonDown(0))
            {
                isAttacking = true;
                if(isMelee)
                {
                    anim.SetTrigger("Attack");
                    weaponAnim.SetTrigger("Attack");
                }
                else
                {
                    anim.SetTrigger("Shoot");
                    weaponAnim.SetTrigger("Shoot");
                }
            }
        }

        anim.SetBool("Moving", isMoving);
        anim.SetBool("Crouching", isCrouching);
        weaponAnim.SetBool("Moving", isMoving);
        weaponAnim.SetBool("Crouching", isCrouching);
        weaponAnim.SetInteger("Ammo", ammo);
    }

    /// <summary>
    /// Interprets status flags.
    /// </summary>
    private void InputHandler()
    {
        rb.velocity = isMoving ? 
            new Vector2(moveSpeed, 0) : 
            Vector2.zero;
        
        if(isAttacking)
        {
            if(isMelee)
                SwingWeapon();
            else
                ShootWeapon();
        }
    }

    /// <summary>
    /// Applies weapon to whatever is in front of player.
    /// </summary>
    private void SwingWeapon()
    {

    }

    /// <summary>
    /// Applies weapon to whatever is ahead of player.
    /// </summary>
    private void ShootWeapon()
    {
        
        ammo = Mathf.Max(ammo-1, 0);
    }

    /// <summary>
    /// Ends reload state 0=incomplete 1=complete
    /// </summary>
    private void EndReload(int status)
    {
        isReloading = false;
        if(status > 0)
        {
            ammo = rangedWeapon.ammo;
            Debug.Log($"Successful Reload!");
        }
    }

    /// <summary>
    /// Ends switchweapon state 0=incomplete 1=complete
    /// </summary>
    private void EndSwitchWeapon(int status)
    {
        isSwitchingWeapons = false;
        if(status > 0)
        {
            isMelee = !isMelee;
            UpdateEquippedWeaponAnims();
            Debug.Log($"Successful Switch Weapon");
        }
    }

    /// <summary>
    /// Updates animations to match currently equipped weapons.
    /// fullUpdate - New weapon, update all animations
    /// </summary>
    private void UpdateEquippedWeaponAnims(bool newWeapon=false)
    {
        if(isMelee)
        {
            weaponAnimOverrides["standIdle"] = meleeWeapon?.standIdle;
            weaponAnimOverrides["crouchIdle"] = meleeWeapon?.crouchIdle;
            weaponAnimOverrides["run"] = meleeWeapon?.run;
            weaponAnimOverrides["standUnequip"] = meleeWeapon?.standUnequip;
            weaponAnimOverrides["crouchUnequip"] = meleeWeapon?.crouchUnequip;

            weaponAnimOverrides["standEquip"] = rangedWeapon?.standEquip;
            weaponAnimOverrides["crouchEquip"] = rangedWeapon?.crouchEquip;
        }
        else
        {
            weaponAnimOverrides["standIdle"] = rangedWeapon?.standIdle;
            weaponAnimOverrides["crouchIdle"] = rangedWeapon?.crouchIdle;
            weaponAnimOverrides["run"] = rangedWeapon?.run;
            weaponAnimOverrides["standUnequip"] = rangedWeapon?.standUnequip;
            weaponAnimOverrides["crouchUnequip"] = rangedWeapon?.crouchUnequip;

            weaponAnimOverrides["standEquip"] = meleeWeapon?.standEquip;
            weaponAnimOverrides["crouchEquip"] = meleeWeapon?.crouchEquip;
        }

        if(newWeapon)
        {
            weaponAnimOverrides["standAttack"] = meleeWeapon?.standAttack;
            weaponAnimOverrides["crouchAttack"] = meleeWeapon?.crouchAttack;
            weaponAnimOverrides["standShoot"] = rangedWeapon?.standShoot;
            weaponAnimOverrides["crouchShoot"] = rangedWeapon?.crouchShoot;
            weaponAnimOverrides["standEmpty"] = rangedWeapon?.standEmpty;
            weaponAnimOverrides["crouchEmpty"] = rangedWeapon?.crouchEmpty;
            weaponAnimOverrides["standReload"] = rangedWeapon?.standReload;
            weaponAnimOverrides["crouchReload"] = rangedWeapon?.crouchReload;
        }

        weaponAnimOverCont.ApplyOverrides(weaponAnimOverrides);
    }
}
