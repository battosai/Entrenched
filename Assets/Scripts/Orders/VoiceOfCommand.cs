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
    /// Order completing cooldown event.
    /// </summary>
    public event Action OnOrderAvailable;

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

        #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            GameState.instance.ui.touchControlToButtons["IssueOrder"].interactable = false;
        #endif

        commissarAnim.SetTrigger("Enter");
        orderAnim.SetTrigger("Enter");
        OnOrderIssued.Invoke(order);
    }

    /// <summary>
    /// Cleans up the order effects for completion.
    /// </summary>
    public void End()
    {
        StartCoroutine(Cooldown());
        commissarAnim.SetTrigger("Exit");
        orderAnim.SetTrigger("Exit");
    }

    // ------------------------------- Data ------------------------------------

    /// <summary>
    /// Distance the player must traverse to activate the ability.
    /// </summary>
    private const int distanceRequired_ft = 150;

    /// <summary>
    /// Animator for commissar.
    /// </summary>
    private Animator commissarAnim;

    /// <summary>
    /// Animator for the commissar's order.
    /// </summary>
    private Animator orderAnim;

    /// <summary>
    /// Override animator for the commissar's order.
    /// </summary>
    private AnimatorOverrideController orderAnimOverride;

    /// <summary>
    /// Override animation clips for the commissar's order.
    /// </summary>
    private AnimationClipOverrides orderAnimOverrideClips;

    /// <summary>
    /// Animation clips for each order.
    /// </summary>
    private Dictionary<string, AnimationClip> orderClips;

    // ------------------------------ Methods ----------------------------------

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        commissarAnim = GetComponent<Animator>();
        orderAnim = transform.Find("Order").GetComponent<Animator>();

        // TODO: Use these to swap out the commissar's order animations based on
        // what order is being used
        orderAnimOverride = 
            new AnimatorOverrideController(orderAnim.runtimeAnimatorController);
        orderAnimOverrideClips = 
            new AnimationClipOverrides(orderAnimOverride.overridesCount);

        orderAnim.runtimeAnimatorController = orderAnimOverride;
        orderAnimOverride.GetOverrides(orderAnimOverrideClips);
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        StartCoroutine(Cooldown());
    }

    /// <summary>
    /// Initiates a cooldown period where this ability is unavailable until its
    /// requirement is met.
    /// </summary>
    private IEnumerator Cooldown()
    {
        available = false;

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

        #if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            GameState.instance.ui.touchControlToButtons["IssueOrder"].interactable = true;
        #endif

        available = true;
        OnOrderAvailable?.Invoke();
    }
}