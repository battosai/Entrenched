using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {CULTIST};
public class Enemy : MonoBehaviour
{

    //stats
    public EnemyType type;
    public int powerLevel;
    public int maxWounds;
    //TODO:
    //determine how to utilize toughness stat
    //NEED: design work
    public int toughness;
    public int moveSpeed;
    public float aggroRange;
    [Header("Combat")]
    public bool isMelee;
    public float meleeRange;
    public float meleeCooldown;
    public float rangedRange;
    public float rangedCooldown;

    //state
    public bool isDead {get; private set;}
    public bool isAlerted;
    public bool isAttacking;
    private int wounds;

    //components
    public StateMachine stateMachine {get; private set;}
    public Rigidbody2D rb {get; private set;}
    public Animator anim {get; private set;}
    public SpriteRenderer rend {get; private set;}
    private Collider2D hitbox;

    //events
    public delegate void Wounded(int dmg);
    public Wounded OnWounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        stateMachine = GetComponent<StateMachine>();
    }

    private void Start()
    {
        wounds = maxWounds;
        isDead = false;
        OnWounded += TakeDamage;

        Krieger.OnDeath += Freeze;

        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>()
        {
            {typeof(IdleState), new IdleState(this)},
            {typeof(AlertState), new AlertState(this)},
            {typeof(ChaseState), new ChaseState(this)},
            {typeof(AttackState),  new AttackState(this)}
        };
        stateMachine.SetStates(states);
    }

    private void Update()
    {
        anim.SetBool("Moving", rb.velocity.magnitude > 0);
    }

    /// <summary>
    /// Reset stats/components and disable object.
    /// </summary>
    public void Cleanup()
    {
        //stats
        isDead = false;
        isAlerted = false;
        isAttacking = false;
        wounds = maxWounds;
        stateMachine.Reset();

        //components
        anim.enabled = true;
        this.gameObject.layer = LayerMask.NameToLayer("Enemies");

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Animation Event: Move after alert.
    /// </summary>
    private void Alert()
    {
        isAlerted = false;
        rb.velocity = Vector2.left * moveSpeed;
    }

    /// <summary>
    /// Checks if the player is in range of our attacks.
    /// </summary>
    public bool InRange()
    {
        float range2 = Mathf.Pow(
            isMelee ? meleeRange : rangedRange, 
            2);

        Vector3 dist = Krieger.instance.transform.position - transform.position;
        float dist2 = Mathf.Pow(dist.magnitude, 2);

        return dist2 <= range2;
    }

    /// <summary>
    /// Animation Event: End attack state and initiate cooldown.
    /// </summary>
    private void EndAttack()
    {
        float cd = isMelee ? meleeCooldown : rangedCooldown;
        if(cd > 0)
            StartCoroutine(Cooldown(cd));
        else
            isAttacking = false;
    }

    /// <summary>
    /// Delay ending attack state.
    /// </summary>
    private IEnumerator Cooldown(float cooldown)
    {
        float t = Time.time;
        while(Time.time - t < cooldown)
            yield return null;
        isAttacking = false;
    }

    /// <summary>
    /// Updates that need to take place before death clip plays.
    /// </summary>
    private void StartDeath()
    {
        isDead = true;
        rb.velocity = Vector3.zero;
        this.gameObject.layer = LayerMask.NameToLayer("Corpses");
    }

    /// <summary>
    /// Animation Event: After death clip to finalize.
    /// </summary>
    private void EndDeath()
    {
        anim.enabled = false;
        GameState.EnemyDefeated(this);
    }

    /// <summary>
    /// Subscriber to Wounded event.
    /// </summary>
    private void TakeDamage(int dmg)
    {
        if(!isDead)
        {
            wounds = Mathf.Max(0, wounds-dmg);

            //TODO:
            //set an anim param for taking a hit
            //NEEDS: animation work

            if(wounds <= 0)
            {
                StartDeath();
                anim.SetTrigger("Die");
            }
        }
    }

    /// <summary>
    /// Subscriber to Krieger OnDeath event. Freeze enemy state.
    /// </summary>
    private void Freeze()
    {
        anim.speed = 0f;
        rb.velocity = Vector3.zero;
    }
}