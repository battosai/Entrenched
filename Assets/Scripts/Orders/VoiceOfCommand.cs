// Standard library includes
using System;
using System.Collections;
using System.Collections.Generic;

// Unity includes
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ability to give out commands.
/// </summary>
public class VoiceOfCommand : MonoBehaviour
{
    // -------------------------- Editor Settings ------------------------------

    // ----------------------------- Interface ---------------------------------

    /// <summary>
    /// Order being issued event.
    /// Order-specific effects should listen to this event.
    /// </summary>
    public event Action<string> OnOrderIssued;

    /// <summary>
    /// Whether or not the order can be used.
    /// </summary>
    public bool available {get; private set;}

    /// <summary>
    /// Entry point for the player issuing the order.
    /// </summary>
    public void Issue(string order)
    {
        Debug.Assert(available == true);

        if (orderSprites.ContainsKey(order) == false)
        {
            orderSprites.Add(
                order,
                Resources.Load<Sprite>($"T_{order}"));
        }

        orderRend.sprite = orderSprites[order];
        commissarAnim.SetTrigger("Enter");

        OnOrderIssued.Invoke(order);
    }

    /// <summary>
    /// Cleans up the order effects for completion.
    /// </summary>
    public void End()
    {
        StartCoroutine(Cooldown());
        commissarAnim.SetTrigger("Exit");
    }

    // ------------------------------- Data ------------------------------------

    /// <summary>
    /// Distance the player must traverse to activate the ability.
    /// </summary>
    private const int distanceRequired_ft = 20;

    /// <summary>
    /// Animator for commissar.
    /// </summary>
    private Animator commissarAnim;

    /// <summary>
    /// Renderer displaying the name of the order in text form.
    /// </summary>
    private SpriteRenderer orderRend;

    /// <summary>
    /// Order text sprite.
    /// </summary>
    private Dictionary<string, Sprite> orderSprites;

    // ------------------------------ Methods ----------------------------------

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        commissarAnim = GetComponent<Animator>();
        orderRend = transform.Find("Order").GetComponent<SpriteRenderer>();
        orderSprites = new Dictionary<string, Sprite>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        orderSprites.Add(
            "OrderReady",
            Resources.Load<Sprite>($"T_OrderReady"));

        StartCoroutine(Cooldown());
    }

    /// <summary>
    /// Initiates a cooldown period where this ability is unavailable until its
    /// requirement is met.
    /// </summary>
    private IEnumerator Cooldown()
    {
        available = false;
        orderRend.enabled = false;

        // Make sure the game has started before counting down
        while (GameState.instance.isReady == false)
        {
            yield return null;
        }

        int start_ft = Utils.GetDistanceTraversed();

        while (Utils.GetDistanceTraversed() - start_ft < distanceRequired_ft)
        {
            yield return null;
        }

        available = true;
        orderRend.enabled = true;
        orderRend.sprite = orderSprites["OrderReady"];
    }
}