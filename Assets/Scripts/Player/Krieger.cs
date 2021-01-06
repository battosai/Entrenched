using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Krieger : MonoBehaviour
{
    public static Krieger instance;

    //player settings
    public float moveSpeed;
    public int clips;
    public int maxClips;
    public int ammoInClip {get; private set;}

    //state
    public bool isMoving;
    public bool isCrouching;
    public bool isCharging;
    private float chargeStartTime;
    public bool isAttacking;
    private float lastMeleeAttackTime;
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
            ammoInClip = _rangedWeapon.clipSize;
            UpdateEquippedWeaponAnims(newWeapon:true);
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
            UpdateEquippedWeaponAnims(newWeapon:true);
        }
    }

    //events
    public delegate void Wounded();
    public Wounded OnWounded;
    public event Action OnDeath;

    private void Awake() 
    {
        //always replace instance bc we know
        //there will only be one instance active at a time
        instance = this;

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

    //TODO:
    //mobile input reading
    //NEED: do it
    /// <summary>
    /// Reads keyboard input to manipulate status flags.
    /// Mobile Inputs:
    /// - hold down a point (no swipe) on the right side to move
    /// - swipe down and hold on right side to crouch
    /// - tap or hold (for charging guns) on left side to shoot/attack
    /// - swipe up to reload
    /// - swipe to left or right to switch weapons
    /// </summary>
    private void ReadInput()
    {
        //hold downs
        isMoving = false;
        isCrouching = false;
        isAttacking = false;

        if(isDead)
            return;

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
                    if(!isMelee && 
                        ammoInClip < rangedWeapon.clipSize &&
                        clips > 0)
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
                if(isMelee)
                {
                    if(Time.time - lastMeleeAttackTime > meleeWeapon.cooldown)
                    {
                        isAttacking = true;
                        lastMeleeAttackTime = Time.time;
                        anim.SetTrigger("Attack");
                        weaponAnim.SetTrigger("Attack");
                    }
                }
                else
                {
                    chargeStartTime = Time.time;
                }
            }
            if(Input.GetMouseButtonUp(0) && !isMelee)
            {
                if(Time.time - chargeStartTime > rangedWeapon.chargeTime)
                {
                    isAttacking = true;
                    anim.SetTrigger("Shoot");
                    weaponAnim.SetTrigger("Shoot");
                }
            }
        }

        anim.SetBool("Moving", isMoving);
        anim.SetBool("Crouching", isCrouching);
        weaponAnim.SetBool("Moving", isMoving);
        weaponAnim.SetBool("Crouching", isCrouching);
        weaponAnim.SetInteger("Ammo", ammoInClip);
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
            UseWeapon(
                isMelee ?
                (Weapon)meleeWeapon :
                (Weapon)rangedWeapon);
    }

    /// <summary>
    /// Applies weapon to whatever is ahead of player.
    /// </summary>
    private void UseWeapon(Weapon weapon)
    {
        //make sure gun has ammoInClip
        if(!isMelee)
        {
            if(ammoInClip == 0)
                return;
            ammoInClip = Math.Max(ammoInClip-1, 0);
        }

        int mask = LayerMask.GetMask("Enemies");
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position, 
            Vector2.right,
            weapon.range,
            mask);
        
        if(hits.Length > 0)
        {
            int wounds = Math.Min(1+weapon.ap, hits.Length);
            for(int i = 0; i < wounds; i++)
            {
                hits[i].collider.GetComponent<Enemy>().OnWounded?.Invoke(weapon.dmg);
            }
        }
    }

    /// <summary>
    /// Animation Event: Ends reload state.
    /// </summary>
    private void EndReload()
    {
        isReloading = false;
        clips--;
        ammoInClip = rangedWeapon.clipSize;
    }

    /// <summary>
    /// Animation Event: Ends switchweapon state 0=incomplete 1=complete
    /// </summary>
    private void EndSwitchWeapon()
    {
        isSwitchingWeapons = false;
        isMelee = !isMelee;
        UpdateEquippedWeaponAnims();
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
    /// Subscriber to OnWounded event.
    /// </summary>
    private void TakeDamage()
    {
        if(!isDead)
        {
            isDead = true;
            anim.SetTrigger("Die");

            //death animation is entirely on torso rend
            legsRend.enabled = false;
            weaponRend.enabled = false;

            OnDeath?.Invoke();
        }
    }

    /// <summary>
    /// Animation Event: Freeze player state.
    /// </summary>
    private void EndDeath()
    {
        anim.speed = 0f;
    }
}
