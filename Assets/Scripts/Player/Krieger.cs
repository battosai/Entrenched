using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Krieger : MonoBehaviour
{
    public static Krieger instance;

    //player settings
    public float moveSpeed;
    public int ammo;
    public float weaponRange;

    //state
    public bool isMoving;
    public bool isCrouching;
    public bool isAttacking;
    public bool isReloading;
    public bool isSwitchingWeapons;
    public bool isMelee;
    public bool isDead;

    //krieger components
    private Rigidbody2D rb;
    private Collider2D hitbox;
    private Animator anim;
    private SpriteRenderer torsoRend;
    private SpriteRenderer legsRend;

    //weapon components
    //TODO:
    //starting weapons should probably be read in from a pre-game scene
    //NEEDS: a pre-game scene 
    public static string startingRangedWeapon;
    public static string startingMeleeWeapon;
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

    //events
    public delegate void Wounded();
    public static Wounded OnWounded;

    private void Awake() 
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(this.gameObject);
        Debug.Assert(armory != null);
        armory.Define();

        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
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
        isDead = false;

        //equip starting weapons
        rangedWeapon = String.IsNullOrEmpty(startingRangedWeapon) ?
            armory.ranged["Lasgun"] :
            armory.ranged[startingRangedWeapon];
        meleeWeapon = String.IsNullOrEmpty(startingMeleeWeapon) ?
            armory.melee["Shovel"] :
            armory.melee[startingMeleeWeapon];

        OnWounded += TakeDamage;
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
            UseWeapon();
    }

    /// <summary>
    /// Applies weapon to whatever is ahead of player.
    /// </summary>
    private void UseWeapon()
    {
        //make sure gun has ammo
        if(!isMelee && ammo == 0)
            return;

        int dmg = isMelee ? meleeWeapon.dmg : rangedWeapon.dmg;
        float range = isMelee ? meleeWeapon.range : rangedWeapon.range;

        int mask = LayerMask.GetMask("Enemies");
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, 
            Vector2.right,
            range,
            mask);
        
        if(hit.collider != null)
        {
            hit.collider.GetComponent<Enemy>().OnWounded?.Invoke(dmg);
        }
        
        ammo = Mathf.Max(ammo-1, 0);
    }

    /// <summary>
    /// Animation Event: Ends reload state.
    /// </summary>
    private void EndReload()
    {
        isReloading = false;
        ammo = rangedWeapon.ammo;
        Debug.Log($"Successful Reload!");
    }

    /// <summary>
    /// Animation Event: Ends switchweapon state 0=incomplete 1=complete
    /// </summary>
    private void EndSwitchWeapon()
    {
        isSwitchingWeapons = false;
        isMelee = !isMelee;
        UpdateEquippedWeaponAnims();
        Debug.Log($"Successful Switch Weapon");
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

    /// <summary>
    /// Called by enemies when they hit the player.
    /// </summary>
    private void TakeDamage()
    {
        if(!isDead)
        {
            isDead = true;
            //TODO:
            //set an anim param for taking a hit
            //NEEDS: animation work
        }
    }
}
