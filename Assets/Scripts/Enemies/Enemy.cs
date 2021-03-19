using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random=UnityEngine.Random;

public enum EnemyType {CULTIST, POSSESSED, BELCHER};
public class Enemy : MonoBehaviour
{
    //stats
    public EnemyType type;
    public int powerLevel;
    public int maxWounds;
    public int moveSpeed;
    public float aggroRange;
    [Header("Combat")]
    public bool isMelee;
    public float meleeRange;
    public float meleeCooldown;
    public float rangedRange;
    public float rangedCooldown;
    public float projectileSpeed;
    [Header("Sounds")]
    public AudioClip[] walks;
    public AudioClip[] attacks;
    public AudioClip death;

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
    private AudioSource audioSource;

    //events
    public delegate void Wounded(int dmg);
    public Wounded OnWounded;
    public event Action<Vector3> OnDeath;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        stateMachine = GetComponent<StateMachine>();
    }

    private void Start()
    {
        wounds = maxWounds;
        isDead = false;
        OnWounded += TakeDamage;
        OnDeath += RollDrops;

        Krieger.instance.OnDeath += Freeze;

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
    /// Called by Scroller and as Animation Event at end of death anims.
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
        if(isDead)
            return;

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
    /// Animation Event: Functional act of attacking. Called on specific frame of attack animation.
    /// </summary>
    private void Attack()
    {
        if(!isMelee)
            return;

        int mask = LayerMask.GetMask("Player");
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, 
            Vector2.left,
            meleeRange,
            mask);
        
        if(hit.collider != null)
            Krieger.instance.OnWounded?.Invoke();
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
    /// Animation Event: Trigger projectile spawn.
    /// </summary>
    private void Shoot()
    {
        Projectile.Spawn(
            transform.position,
            projectileSpeed);
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
        StartCoroutine(Decay());
    }

    /// <summary>
    /// Timer for how long a dead body should stay on screen before going away.
    /// </summary>
    private IEnumerator Decay()
    {
        float decayTime = 5f;
        float timer = Time.time;
        while(Time.time - timer < decayTime) 
        {
            //stop waiting if no longer dead
            //scroller will call Cleanup()
            if(!isDead)
                break;
            yield return null;
        }

        //bodies will finish death anims
        if(isDead)
            anim.enabled = true;
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
                OnDeath?.Invoke(transform.position);
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
        stateMachine.Pause();
    }

    /// <summary>
    /// Decide if enemy drops loot.
    /// </summary>
    private void RollDrops(Vector3 position)
    {
        float roll = Random.Range(0f, 1f);

        if(roll < Ammo.dropChance*maxWounds)
        {
            Ammo.Spawn(transform.position);
            AudioManager.PlayOne(audioSource, AudioManager.instance.ammoDrops);
        }
    }
}