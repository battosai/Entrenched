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

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    private void Start() {}

    private void Update()
    {
        InputHandler();
    }

    private void InputHandler()
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
        if(Input.GetMouseDown(1))
            isSwitchingWeapons = true;
        if(isSwitchingWeapons)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                if(!isMelee)
                    isReloading = true;
            }
            if(Input.GetMouseDown(0))
                isAttacking = true;
        }
    }
}
