using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Type {CULTIST};

    //stats
    public Type type;
    public int powerLevel;
    public int maxWounds;
    //TODO:
    //determine how to utilize toughness stat
    //NEED: design work
    public int toughness;
    public int dmg;
    public int moveSpeed;

    //state
    public bool isDead {get; private set;}
    private bool isAlerted;
    private bool isMelee;
    private bool isAttacking;
    private int wounds;

    //components
    private Rigidbody2D rb;
    private Collider2D hitbox;
    //TODO:
    //need some other trigger collider that will tell the enemy they are in range
    //NEEDS: design work, this isn't concrete yet, could just use distance calcs
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
        wounds = maxWounds;
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
    /// Reset stats/components and disable object.
    /// </summary>
    public void Cleanup()
    {
        //TODO:
        //not sure what to do yet if enemies reach scroller and are alive
        //NEEDS: design work
        if(!isDead)
            return;

        //stats
        isDead = false;
        wounds = maxWounds;

        //components
        anim.enabled = true;
        hitbox.enabled = true;

        this.gameObject.SetActive(false);
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
        Gamestate.EnemyDefeated(this);
        //TODO:
        //maybe do any stat tracking updates here
        //NEEDS: a stat tracker 
    }

    /// <summary>
    /// Subscriber to Wounded event.
    /// </summary>
    private void TakeDamage(int dmg)
    {
        wounds = Mathf.Max(0, wounds-dmg);
        //TODO:
        //set an anim param for taking a hit
        //NEEDS: animation work
    }
}