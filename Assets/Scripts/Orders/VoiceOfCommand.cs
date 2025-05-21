// Standard library includes
using System;
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
    public bool available
    {
        get
        {
            return Time.time - lastOrder_s >= cooldown_s;
        }
    }

    /// <summary>
    /// Entry point for the player issuing the order.
    /// </summary>
    public void Issue(string order)
    {
        Debug.Assert(available == true);
        lastOrder_s = Time.time;

        if (orderSprites.ContainsKey(order) == false)
        {
            orderSprites.Add(
                order,
                Resources.Load<Sprite>($"T_{order}"));
        }

        orderRend.sprite = orderSprites[order];
        orderRend.enabled = true;
        commissarAnim.SetTrigger("Enter");

        OnOrderIssued.Invoke(order);
    }

    /// <summary>
    /// Cleans up the order effects for completion.
    /// </summary>
    public void End()
    {
        orderRend.enabled = false;
        commissarAnim.SetTrigger("Exit");
    }

    // ------------------------------- Data ------------------------------------

    /// <summary>
    /// Time it takes before this order can be used again.
    /// </summary>
    private float cooldown_s;

    /// <summary>
    /// Timestamp for when the last order was issued.
    /// </summary>
    private float lastOrder_s;

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
        cooldown_s = 20f;
        lastOrder_s = Time.time - cooldown_s;
        orderRend.enabled = false;
    }
}