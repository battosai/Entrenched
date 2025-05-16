// Standard library includes
using System;

// Unity includes
using UnityEngine;

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

    [Header("Audio Clips")]

    /// <summary>
    /// Trench whistle sound.
    /// </summary>
    public AudioClip trenchWhistle;

    /// <summary>
    /// Group of people running sound.
    /// </summary>
    public AudioClip runningGroup;

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

    // ------------------------------ Methods ----------------------------------

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        body = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        gameObject.SetActive(false);
        Krieger.instance.voice.OnOrderIssued += Charge;
        Krieger.instance.OnDeath += Freeze;
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
            other.GetComponent<Enemy>().OnWounded?.Invoke(dmg);
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

        body.velocity = Vector2.right * speed;

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
    /// Subscriber to Krieger OnDeath event.
    /// Freeze charge object.
    /// </summary>
    private void Freeze()
    {
        body.velocity = Vector3.zero;

        foreach (Animator anim in animators)
        {
            anim.speed = 0f;
        }
    }
}