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
    private SpriteRenderer rend;
    private Animator anim;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start() {}

    private void Update()
    {
        ReadInput();
        InputHandler();
        AnimatorHandler();
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
            isSwitchingWeapons = true;
        if(!isSwitchingWeapons)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                if(!isMelee)
                    isReloading = true;
            }
            if(Input.GetMouseButton(0))
                isAttacking = true;
        }
    }

    /// <summary>
    /// Interprets status flags.
    /// </summary>
    private void InputHandler()
    {
        rb.velocity = isMoving ? new Vector2(moveSpeed, 0) : Vector2.zero;
    }

    private void AnimatorHandler()
    {
        anim.SetBool("Moving", isMoving);
        anim.SetBool("Crouching", isCrouching);
        anim.SetBool("Shooting", isAttacking && !isMelee);
        anim.SetBool("Attacking", isAttacking && isMelee);
    }
}
