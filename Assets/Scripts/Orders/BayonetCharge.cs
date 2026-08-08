// Standard library includes
using System;

// Unity includes
using UnityEngine;
using UnityEngine.Assertions;

// Aliases
using Random=UnityEngine.Random;

/// <summary>
/// The group of kriegers charging from the Fix Bayonets order.
/// </summary>
public class BayonetCharge : MonoBehaviour
{
    // -------------------------- Editor Settings ------------------------------

    // ----------------------------- Interface ---------------------------------

    [Header("Settings")]

    /// <summary>
    /// Speed of the bayonet charge across the screen.
    /// </summary>
    [SerializeField]
    private float speed;

    /// <summary>
    /// Number of people walking to simulate audio.
    /// </summary>
    [SerializeField]
    private int walkers;

    /// <summary>
    /// Time between footstep sounds for a single walker.
    /// </summary>
    [SerializeField]
    private float baseStepInterval_s;

    /// <summary>
    /// Variance applied to each walker's footstep sound interval.
    /// </summary>
    [SerializeField]
    private float stepIntervalJitter_s;

    [Header("Audio Clips")]

    /// <summary>
    /// Trench whistle sound.
    /// </summary>
    public AudioClip trenchWhistle;

    // ------------------------------- Data ------------------------------------

    /// <summary>
    /// Order name that triggers this.
    /// </summary>
    private const string order = "FixBayonets";

    /// <summary>
    /// Damage this deals to enemeis.
    /// </summary>
    private const int dmg = 2;

    /// <summary>
    /// Timers for each simulated walker.
    /// </summary>
    private float[] walkerTimers_s;

    /// <summary>
    /// Step intervals for each simulated walker.
    /// </summary>
    private float[] walkerIntervals_s;

    /// <summary>
    /// Timer for bayonet swings.
    /// </summary>
    private float swingTimer_s;

    /// <summary>
    /// List of krieger animators.
    /// </summary>
    private Animator[] animators;

    /// <summary>
    /// Trigger hitbox for killing.
    /// </summary>
    private Collider2D hitbox;

    /// <summary>
    /// Rigidbody component.
    /// </summary>
    private Rigidbody2D body;

    /// <summary>
    /// AudioSource component.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Cached reference to Krieger's powersword.
    /// </summary>
    private MeleeWeapon kriegerSword;

    // ------------------------------ Methods ----------------------------------

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        // Cache component references
        hitbox = GetComponent<Collider2D>();
        body = GetComponent<Rigidbody2D>();
        audioSource =  GetComponent<AudioSource>();
        animators = GetComponentsInChildren<Animator>();
        Assert.IsNotNull(hitbox);
        Assert.IsNotNull(body);
        Assert.IsNotNull(audioSource);
        Assert.IsTrue(animators.Length > 0);
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        gameObject.SetActive(false);

        Assert.IsNotNull(Krieger.instance);
        Krieger.instance.voice.OnOrderIssued += Charge;
        Krieger.instance.OnDeath += Freeze;

        kriegerSword = Krieger.instance.armory.melee["Powersword"];
        Assert.IsNotNull(kriegerSword);

        // Initialize audio timers
        Assert.IsTrue(walkers > 0);
        walkerTimers_s = new float[walkers];
        walkerIntervals_s = new float[walkers];

        for(int ii = 0;
            ii < walkers;
            ii++)
        {
            walkerTimers_s[ii] = Random.Range(
                0f,
                baseStepInterval_s);

            walkerIntervals_s[ii] = GetRandomWalkerInterval();
        }

       swingTimer_s = 0; 
    }

    /// <summary>
    /// Cleanup.
    /// </summary>
    private void OnDestroy()
    {
        Krieger.instance.voice.OnOrderIssued -= Charge;
        Krieger.instance.OnDeath -= Freeze;
    }

    /// <summary>
    /// Every frame update loop.
    /// </summary>
    private void Update()
    {
        // Simulate group footstep sounds
        Assert.IsNotNull(Krieger.instance);
        Assert.IsNotNull(Krieger.instance.walks);
        Assert.IsTrue(Krieger.instance.walks.Length > 0);

        for(int ii = 0;
            ii < walkerTimers_s.Length;
            ii++)
        {
            walkerTimers_s[ii] += Time.deltaTime;
            if (walkerTimers_s[ii] < walkerIntervals_s[ii])
            {
                continue;
            }

            walkerTimers_s[ii] = 0f;
            walkerIntervals_s[ii] = GetRandomWalkerInterval();
            AudioManager.PlayOneClip(
                audioSource,
                Krieger.instance.walks,
                0.5f);
        }

        // Bayonet swing sound
        Assert.IsNotNull(kriegerSword);
        Assert.IsNotNull(kriegerSword.swings);

        const float swingInterval_s = 0.25f;
        swingTimer_s += Time.deltaTime;
        if (swingTimer_s < swingInterval_s)
        {
            return;
        }

        swingTimer_s = 0f;
        AudioManager.PlayOneClip(
            audioSource,
            kriegerSword.swings,
            0.4f);
    }

    /// <summary>
    /// Trigger collision handler.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null)
        {
            return;
        }

        // Kill enemies
        if (other.tag == "Enemies")
        {
            Enemy enemy = other.GetComponent<Enemy>();
            Assert.IsNotNull(enemy);
            enemy.OnWounded.Invoke(dmg);

            Assert.IsNotNull(kriegerSword);
            Assert.IsNotNull(kriegerSword.hits);
            AudioManager.PlayOneClip(
                audioSource,
                kriegerSword.hits);
        }
    }

    /// <summary>
    /// Collider exiting this trigger handler.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        // Only care about EndOfScreen
        if (other == null ||
            other.tag == "Enemies")
        {
            return;
        }

        // Pause spawner for a bit so that 
        // the screen clear is worthwhile
        GameState.instance.OnSpawnerPause.Invoke(5f);

        // End bayonet charge
        Krieger.instance.EndIssueOrder();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Voice of command listener that initiates the bayonet charge.
    /// </summary>
    private void Charge(string issuedOrder)
    {
        // Has to be the right order
        if (issuedOrder != order)
        {
            return;
        }

        // Make chargers active, place them, and give speed
        // (must be before setting velocity)
        gameObject.SetActive(true);

        transform.position = 
            Krieger.instance.transform.position + 
            Vector3.left * 40;

        body.linearVelocity = Vector2.right * speed;

        // Randomize the start point in the animation
        // so that they are not in unison
        foreach (Animator animator in animators)
        {
            animator.Play(
                0, 
                -1, 
                UnityEngine.Random.value);
        }

        AudioManager.PlayClip(
            Krieger.instance.audioSource, 
            trenchWhistle);
    }

    /// <summary>
    /// Get a randomized step interval.
    /// </summary>
    private float GetRandomWalkerInterval()
    {
        float interval_s = baseStepInterval_s + Random.Range(
            -stepIntervalJitter_s, 
            stepIntervalJitter_s);

        return Mathf.Max(
            0.05f, // Safety clamp
            interval_s);
    }

    /// <summary>
    /// Subscriber to Krieger OnDeath event.
    /// Freeze charge object.
    /// </summary>
    private void Freeze()
    {
        body.linearVelocity = Vector3.zero;

        foreach (Animator anim in animators)
        {
            anim.speed = 0f;
        }
    }
}