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

    //components
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer torsoRend;
    private SpriteRenderer legsRend;
    private SpriteRenderer weaponRend;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        torsoRend = transform.Find("Torso").GetComponent<SpriteRenderer>();
        legsRend = transform.Find("Legs").GetComponent<SpriteRenderer>();
        weaponRend = transform.Find("Weapon").GetComponent<SpriteRenderer>();
    }

    private void Start() {}

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
        if(Input.GetKey(KeyCode.D))    
            isMoving = true;
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
}
