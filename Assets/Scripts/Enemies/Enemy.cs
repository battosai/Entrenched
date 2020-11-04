using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //stats
    public int powerLevel;
    public int wounds;
    public int toughness;
    public int dmg;
    public int moveSpeed;

    //state
    private bool isAlerted;
    private bool isMelee;
    private bool isAttacking;

    //components
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    //events
    public delegate void Wounded(int dmg);
    public Wounded OnWounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        OnWounded += TakeDamage;
    }

    private void Update()
    {
        if(isAttacking)
        {
            if(isMelee)
                anim.SetTrigger("Attack");
            else
                anim.SetTrigger("Shoot");
        }

        //test
        if(Input.GetKeyDown(KeyCode.R))
        {
            //only set trigger if not already alerted
            anim.SetTrigger("Alert");
        }

        anim.SetBool("Moving", rb.velocity.magnitude > 0);
        anim.SetInteger("Wounds", wounds);
    }

    /// <summary>
    /// Animation event function call to move after alert.
    /// </summary>
    private void EndAlert(int status)
    {
        rb.velocity = Vector3.left * moveSpeed;
    }

    /// <summary>
    /// Subscriber to Wounded event.
    /// </summary>
    private void TakeDamage(int dmg)
    {
        wounds = Mathf.Max(0, wounds-dmg);
        //set an anim param for taking a hit
    }
}