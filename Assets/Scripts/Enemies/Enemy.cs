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
    private bool isDead;
    private bool isAlerted;
    private bool isMelee;
    private bool isAttacking;

    //components
    private Rigidbody2D rb;
    private Collider2D hitbox;
    //need some other trigger collider that will tell the enemy they are in range
    private Animator anim;

    //events
    public delegate void Wounded(int dmg);
    public Wounded OnWounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        isDead = false;
        OnWounded += TakeDamage;
    }

    private void Update()
    {
        if(!isDead)
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

            if(wounds <= 0)
            {
                StartDeath();
                anim.SetTrigger("Die");
            }
            anim.SetBool("Moving", rb.velocity.magnitude > 0);
        }
    }

    /// <summary>
    /// Animation Event: Move after alert.
    /// </summary>
    private void Alert()
    {
        rb.velocity = Vector3.left * moveSpeed;
    }

    /// <summary>
    /// Updates that need to take place before death clip plays.
    /// </summary>
    private void StartDeath()
    {
        isDead = true;
        rb.velocity = Vector3.zero;
        hitbox.enabled = false;
    }

    /// <summary>
    /// Animation Event: After death clip to finalize.
    /// </summary>
    private void EndDeath()
    {
        anim.enabled = false;
        //maybe do any stat tracking updates here
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