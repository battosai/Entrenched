using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Krieger : MonoBehaviour
{
    //player settings
    public float moveSpeed;

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
            SwapAnimations(ranged: _rangedWeapon);
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
            SwapAnimations(melee: _meleeWeapon);
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
        if(!isReloading)
            if(Input.GetKey(KeyCode.D))    
                isMoving = true;
        if(!isAttacking)
            if(Input.GetKey(KeyCode.LeftShift))
            {
                isCrouching = true;
                isMoving = false;
            }

        //one taps (and take animation time to reset)
        if(Input.GetMouseButtonDown(1))
        {
            isSwitchingWeapons = true;
            anim.SetTrigger("SwitchWeapon");
        }
        if(!isSwitchingWeapons)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                if(!isMelee)
                {
                    isReloading = true;
                    anim.SetTrigger("Reload");
                }
            }
            if(!isReloading)
                if(Input.GetMouseButtonDown(0))
                {
                    isAttacking = true;
                    if(isMelee)
                        anim.SetTrigger("Attack");
                    else
                        anim.SetTrigger("Shoot");
                }
        }

        anim.SetBool("Moving", isMoving);
        anim.SetBool("Crouching", isCrouching);
    }

    /// <summary>
    /// Interprets status flags.
    /// </summary>
    private void InputHandler()
    {
        rb.velocity = isMoving ? 
            new Vector2(moveSpeed, 0) : 
            Vector2.zero;
    }

    /// <summary>
    /// Updates animations of equipped weapons.
    /// </summary>
    private void SwapAnimations(RangedWeapon ranged=null, MeleeWeapon melee=null)
    {
        if(ranged != null)
        {
            //swap out animations
        }

        if(melee != null)
        {
            //swap out animations
        }
    }

    /// <summary>
    /// Ends reload state 0=incomplete 1=complete
    /// </summary>
    private void EndReload(int status)
    {
        isReloading = false;
        if(status > 0)
        {
            //reset ammo
            Debug.Log($"Successful Reload!");
        }
    }

    /// <summary>
    /// Ends switchweapon state 0=incomplete 1=complete
    /// </summary>
    private void EndSwitchWeapon()
    {
        isSwitchingWeapons = false;
        //do something to trigger weaponAnim to update and match the weapon

        //start unequip anim (which will auto transition to equip anim)
        //  - will need to update the anim clip before the transition
    }
}
