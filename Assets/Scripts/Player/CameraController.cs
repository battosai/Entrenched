using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Rigidbody2D followTarget;
    private float targetWidth;
    private float targetHeight;
    private Vector2 offsetFromTarget;

    public float parallaxEffect;
    private Transform background;
    private Dictionary<string, SpriteRenderer> backgroundElements;

    private Camera cam;

    private void Awake()
    {
        Debug.Assert(followTarget != null);

        cam = GetComponent<Camera>();
        background = transform.Find("Background");
        backgroundElements = new Dictionary<string, SpriteRenderer>();

        //initially position target at bottom left
        Sprite targetSprite = followTarget.transform.Find("Torso").GetComponent<SpriteRenderer>().sprite;
        targetWidth = targetSprite.bounds.size.x;
        targetHeight = targetSprite.bounds.size.y;
        Vector3 currentBottomLeft = cam.ScreenToWorldPoint(new Vector3(0f, 0f, 10f));
        currentBottomLeft += new Vector3(targetWidth/2f, targetHeight/2f, 0f);
        Vector3 move = followTarget.transform.position - currentBottomLeft;
        transform.position += move;
        offsetFromTarget = transform.position - followTarget.transform.position;
    }

    private void Start()
    {
        InitializeBackground();
    }

    /// <summary>
    /// Prepares background objects for scrolling and aligns it.
    /// </summary>
    private void InitializeBackground()
    {
        //align
        Scroller scroller = transform.Find("Scroller").GetComponent<Scroller>();
        Vector3 offset = scroller.chunkA.position - cam.transform.position;
        background.localPosition = offset;

        //make clone and enter into bg dict for scrolling
        //go backwards to not iterate over clones
        for(int i = background.childCount-1; i >= 0; i--)
        {
            Transform element = background.GetChild(i);
            backgroundElements.Add(element.name, element.GetComponent<SpriteRenderer>());

            GameObject clone = Instantiate(element.gameObject, background);
            clone.name = clone.name.Remove(element.name.Length-1) + "B";
            clone.transform.localPosition = element.localPosition + new Vector3(scroller.chunkWidth, 0, 0f);
            backgroundElements.Add(clone.name, clone.GetComponent<SpriteRenderer>());
        }
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
            followTarget.transform.position.x + offsetFromTarget.x, 
            transform.position.y, 
            transform.position.z);
    }

    /// <summary>
    /// Manage parallax effect on background objects.
    /// </summary>
    private void ParallaxBackground()
    {
        //only if we're moving
        if(followTarget.velocity.magnitude > 0) 
        {
            foreach(KeyValuePair<string, SpriteRenderer> pair in backgroundElements)
            {
                //apply parallax effect scaling with sorting layer of rend
                SpriteRenderer element = pair.Value;
                Vector3 move = -followTarget.velocity * 
                    parallaxEffect * 
                    Time.deltaTime * 
                    (element.sortingOrder + 1)/(element.sortingOrder + 2);    
                element.transform.localPosition += move;
            }
        }
    }
}