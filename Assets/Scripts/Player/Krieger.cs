using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Krieger : MonoBehaviour
{
    public static Krieger instance;

    //player settings
    [Header("Stats")]
    public float moveSpeed;
    public int clips;
    public int maxClips;
    public int ammoInClip {get; private set;}

    [Header("Sounds")]
    public AudioClip[] walks;
    public AudioClip[] ammoPickups;

    /// <summary>
    /// Whether or not the player is in the middle of something.
    /// </summary>
    private bool busy
    {
        get
        {
            return isReloading == true ||
                isSwitchingWeapons == true ||
                isIssuingOrder == true;
        }
    }

    public bool isMoving;
    private float moveStartTime;
    public bool isCrouching;
    public bool isCharging;
    private float chargeStartTime;
    public bool isAttacking;
    private float lastMeleeAttackTime;
    public bool isReloading;
    public bool isSwitchingWeapons;
    public bool isMelee;
    public bool isIssuingOrder;
    public bool isDead;

    //krieger components
    private PlayerInput input;
    public AudioSource audioSource {get; private set;}
    private Rigidbody2D rb;
    private Collider2D hitbox;
    public Animator anim {get; private set;}
    public SpriteRenderer torsoRend {get; private set;}
    public SpriteRenderer legsRend {get; private set;}
    public SpriteRenderer ammoRend {get; private set;}

    //weapon components
    public Armory armory;
    private Transform weaponTrans;
    public Animator weaponAnim {get; private set;}
    private AnimatorOverrideController weaponAnimOverCont;
    private AnimationClipOverrides weaponAnimOverrides;
    public SpriteRenderer weaponRend {get; private set;}
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
    public string startingRangedWeapon
    {
        get
        {
            return rangedWeapon == null ? "" : rangedWeapon.name;
        }
        set
        {
            if(armory.ranged.ContainsKey(value))
            {
                rangedWeapon = armory.ranged[value];
            }
        }
    }
    public string startingMeleeWeapon
    {
        get
        {
            return meleeWeapon == null ? "" : meleeWeapon.name;
        }
        set
        {
            if(armory.melee.ContainsKey(value))
            {
                meleeWeapon = armory.melee[value];
            }
        }
    }

    /// <summary>
    /// Ability to give out commands.
    /// </summary>
    public VoiceOfCommand voice {get; private set;}

    //events
    public delegate void Wounded();
    public Wounded OnWounded;
    public event Action OnDeath;
    public event Action OnShoot;

    private void Awake() 
    {
        //always replace instance bc we know
        //there will only be one instance active at a time
        instance = this;

        Debug.Assert(armory != null);
        armory.Define();

        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        torsoRend = transform.Find("Torso").GetComponent<SpriteRenderer>();
        legsRend = transform.Find("Legs").GetComponent<SpriteRenderer>();
        ammoRend = transform.Find("AmmoCount").GetComponent<SpriteRenderer>();

        weaponTrans = transform.Find("Weapon");
        weaponAnim = weaponTrans.GetComponent<Animator>();
        weaponRend = weaponTrans.GetComponent<SpriteRenderer>();

        voice = transform.Find("Commissar").GetComponent<VoiceOfCommand>();

        //anim
        weaponAnimOverCont = 
            new AnimatorOverrideController(weaponAnim.runtimeAnimatorController);
        weaponAnim.runtimeAnimatorController = weaponAnimOverCont;
        weaponAnimOverrides = 
            new AnimationClipOverrides(weaponAnimOverCont.overridesCount);
        weaponAnimOverCont.GetOverrides(weaponAnimOverrides);
    }

    private void Start() 
    {
        input = new PlayerInput();

        //status
        isMelee = false;
        isDead = false;

        OnWounded += TakeDamage;
    }

    /// <summary>
    /// Cleanup.
    /// </summary>
    private void OnDestroy()
    {
        OnWounded -= TakeDamage;
    }

    private void Update()
    {
        if(!GameState.instance.isReady)
            return;

        if(isDead)
            return;

        input.Read();
        InputHandler();
    }

    /// <summary>
    /// Interpret input to manipulate status flags.
    /// </summary>
    private void InputHandler()
    {
        // Hold downs
        isMoving = false;
        isCrouching = false;
        isAttacking = false;

        // Ignore if dead
        if (isDead == true)
        {
            return;
        }

        // Can only move if we aren't reloading/switching
        if (busy == false)
        {
            if (input._move == true)
            {
                isMoving = true;
            }
        }

        // Can only begin crouching if we aren't attacking
        if (isAttacking == false)
        {
            if (input._crouch == true)
            {
                isCrouching = true;
                isMoving = false;
            }
        }

        // One taps (and take animation time to reset)
        if (busy == false)
        {
            // Start issuing order
            if (input._issueOrder == true &&
                voice.available == true)
            {

                isIssuingOrder = true;
                isMoving = false;
                voice.Issue("FixBayonets");
            }

            // Start switching weapons
            else if (input._switch == true &&
                isIssuingOrder == false)
            {
                isSwitchingWeapons = true;
                isMoving = false;
                anim.SetTrigger("SwitchWeapon");
                weaponAnim.SetTrigger("SwitchWeapon");
            }

            // Can reload if we aren't currently switching weapons
            else if (input._reload == true &&
                isSwitchingWeapons == false)
            {
                if (isMelee == false && 
                    ammoInClip < rangedWeapon.clipSize &&
                    clips > 0)
                {
                    AudioManager.PlayOneClip(audioSource, rangedWeapon.reloads);
                    isReloading = true;
                    anim.SetTrigger("Reload");
                    weaponAnim.SetTrigger("Reload");
                }
            }
        }

        // Instant one-ticks
        if (busy == false)
        {
            // TODO: Change non-charging weapons to trigger on KeyDown 

            // Perform melee attack
            if (input._attackRelease == true && 
                isMelee == true)
            {
                if (Time.time - lastMeleeAttackTime > meleeWeapon.cooldown)
                {
                    isAttacking = true;
                    lastMeleeAttackTime = Time.time;
                    anim.SetTrigger("Attack");
                    weaponAnim.SetTrigger("Attack");
                }
            }

            // Start charging ranged weapon
            if (input._attackDown == true && 
                isMelee == false)
            {
                chargeStartTime = Time.time;
                
                if (isMelee == false &&
                    ammoInClip > 0 &&
                    rangedWeapon.chargeTime > 0)
                {
                    float startTime = rangedWeapon.charge.length - rangedWeapon.chargeTime;
                    AudioManager.Play(
                        audioSource, 
                        rangedWeapon.charge,
                        startTime:startTime);
                }
            }

            // Positive charge start time to ensure
            // there was a corresponding mouse down
            // (clicking ready button in weapon selection)
            if (chargeStartTime > 0 && 
                isMelee == false)
            {
                // Release charge
                if (input._attackRelease == true)
                {
                    if (Time.time - chargeStartTime > rangedWeapon.chargeTime)
                    {
                        isAttacking = true;
                        anim.SetTrigger("Shoot");
                        weaponAnim.SetTrigger("Shoot");
                    }

                    chargeStartTime = -1f;

                    if (audioSource.isPlaying == true)
                    {
                        audioSource.Stop();
                    }
                }

                // Hold charge
                else if (input._attackHold == true)
                {
                    if (Time.time - chargeStartTime > rangedWeapon.chargeTime)
                    {
                        if (audioSource.clip != rangedWeapon.fullCharge)
                        {
                            AudioManager.Play(
                                audioSource,
                                rangedWeapon.fullCharge,
                                loop:true,
                                startTime:0f);
                        }
                    }
                }

            }
        }

        if (anim.GetBool("Moving") == false && 
            isMoving == true)
        {
            moveStartTime = Time.time;
        }
        else if(anim.GetBool("Moving") == true && 
            isMoving == false)
        {
            moveStartTime = -1f;
        }

        anim.SetBool("Moving", isMoving);
        anim.SetBool("Crouching", isCrouching);
        weaponAnim.SetBool("Moving", isMoving);
        weaponAnim.SetBool("Crouching", isCrouching);
        weaponAnim.SetInteger("Ammo", ammoInClip);

        if (moveStartTime > 0)
        {
            // Walk anim is 0.5s, each step is half of it
            float stepDelay = 0.25f;

            if (Time.time - moveStartTime > stepDelay)
            {
                AudioManager.PlayOneClip(
                    audioSource, 
                    walks,
                    0.25f);

                moveStartTime = Time.time;
            }
        }

        rb.velocity = isMoving ? 
            new Vector2(moveSpeed, 0) : 
            Vector2.zero;

        hitbox.offset = isCrouching ?
            new Vector2(0, -2) :
            Vector2.zero;
        
        if (isAttacking == true)
        {
            UseWeapon(
                isMelee ?
                (Weapon)meleeWeapon :
                (Weapon)rangedWeapon);
        }
    }

    /// <summary>
    /// Applies weapon to whatever is ahead of player.
    /// </summary>
    private void UseWeapon(Weapon weapon)
    {
        if(isMelee)
        {
            // Currently, the hit sounds for melee weapons includes a swing sound as well in the same clip
            // so we're gonna check to see if there's a hit first and decide what clip to play from there.
            // AudioManager.PlayOneClip(audioSource, meleeWeapon.swings);
        }
        else
        {
            //make sure gun has ammoInClip
            if(ammoInClip == 0)
            {
                AudioManager.PlayOneClip(audioSource, rangedWeapon.dryShots);
                return;
            }
            AudioManager.PlayOneClip(audioSource, rangedWeapon.shots);
            ammoInClip = Math.Max(ammoInClip-1, 0);
            OnShoot?.Invoke();
        }

        int mask = LayerMask.GetMask("Enemies");
        int xOffset = 5;
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position + (Vector3.left * xOffset), 
            Vector2.right,
            weapon.range + xOffset,
            mask);

        if(hits.Length > 0)
        {
            AudioManager.PlayOneClip(audioSource, isMelee ? meleeWeapon.hits : rangedWeapon.hits);
            int wounds = Math.Min(1+weapon.ap, hits.Length);
            for(int i = 0; i < wounds; i++)
            {
                hits[i].collider.GetComponent<Enemy>().OnWounded?.Invoke(weapon.dmg);
            }
        }
        else
        {
            // Currently, the hit sounds for melee weapons includes a swing sound as well in the same clip
            // so we're gonna check to see if there's a hit first and decide what clip to play from there.
            if(isMelee)
                AudioManager.PlayOneClip(audioSource, meleeWeapon.swings);
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

        #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            GameState.instance.ui.touchControlToButtons["Reload"].interactable = !isMelee;
        #endif
    }

    /// <summary>
    /// End the issuing order state.
    /// Can be an Animation Event or just manually 
    /// called depending on how the order functions.
    /// </summary>
    public void EndIssueOrder()
    {
        voice.End();
        isIssuingOrder = false;
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
            rb.velocity = Vector2.zero;

            //death animation is entirely on torso rend
            anim.SetTrigger("Die");
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

    /// <summary>
    /// Utility function to set all 3 of Krieger's renderer layers.
    /// </summary>
    public void SetRendererLayer(string layer)
    {
        torsoRend.sortingLayerName = layer;
        legsRend.sortingLayerName = layer;
        weaponRend.sortingLayerName = layer;
    }
}