// Standard library includes
using System;

// Unity includes
using UnityEngine;

/// <summary>
/// End of screen boundary.
/// </summary>
public class EndOfScreen : MonoBehaviour
{
    // -------------------------- Editor Settings ------------------------------

    // ----------------------------- Interface ---------------------------------

    // ------------------------------- Data ------------------------------------

    /// <summary>
    /// End of screen boundary collider.
    /// </summary>
    private BoxCollider2D coll;

    // ------------------------------ Methods ----------------------------------

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        const float distFromEdge = 0;

        Vector3 rightEdgeOfScreen = Camera.main.ViewportToWorldPoint(
            new Vector3(
                1f,
                0.5f));

        transform.position = 
            rightEdgeOfScreen +
            (Vector3.right * (coll.size.x + distFromEdge));

        coll.size = new Vector2(
            coll.size.x, 
            Camera.main.orthographicSize * 2);
    }
}