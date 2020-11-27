using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;

    public float parallaxEffect;

    private float targetWidth;
    private float targetHeight;
    private Vector2 offsetFromTarget;
    private Camera cam;
    private Transform background;

    private void Awake()
    {
        Debug.Assert(followTarget != null);

        cam = GetComponent<Camera>();
        background = transform.Find("Background");

        //initially position target at bottom left
        Sprite targetSprite = followTarget.Find("Torso").GetComponent<SpriteRenderer>().sprite;
        targetWidth = targetSprite.bounds.size.x;
        targetHeight = targetSprite.bounds.size.y;
        Vector3 currentBottomLeft = cam.ScreenToWorldPoint(new Vector3(0f, 0f, 10f));
        currentBottomLeft += new Vector3(targetWidth/2f, targetHeight/2f, 0f);
        Vector3 move = followTarget.position - currentBottomLeft;
        transform.position += move;
        offsetFromTarget = transform.position - followTarget.position;
    }

    private void Start()
    {
        AlignBackground();
    }

    /// <summary>
    /// Aligns the Background parent object.
    /// </summary>
    private void AlignBackground()
    {
        Scroller scroller = transform.Find("Scroller").GetComponent<Scroller>();
        Vector3 offset = scroller.chunkA.position - cam.transform.position;
        background.localPosition = offset;
    }

    private void Update()
    {
        HorizontalFollow();
        ParallaxBackground();
    }

    /// <summary>
    /// Follows followTarget horizontally.
    /// </summary>
    private void HorizontalFollow()
    {
        transform.position = new Vector3(
            followTarget.position.x + offsetFromTarget.x, 
            transform.position.y, 
            transform.position.z);
    }

    /// <summary>
    /// Manage parallax effect on background objects.
    /// </summary>
    private void ParallaxBackground()
    {

    }
}