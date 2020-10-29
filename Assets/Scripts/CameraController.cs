using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;

    private float targetWidth;
    private float targetHeight;
    private Vector2 offsetFromTarget;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        Debug.Assert(followTarget != null);
    }

    private void Start()
    {
        //initially position target at bottom left
        Sprite targetSprite = followTarget.Find("Torso").GetComponent<SpriteRenderer>().sprite;
        targetWidth = targetSprite.bounds.size.x;
        targetHeight = targetSprite.bounds.size.y;
        Vector3 currentBottomLeft = cam.ScreenToWorldPoint(new Vector3(0f, 0f, 10f));
        currentBottomLeft += new Vector3(targetWidth/2f-2f, targetHeight/2f-1f, 0f);
        Vector3 move = followTarget.position - currentBottomLeft;
        transform.position += move;
        offsetFromTarget = transform.position - followTarget.position;
    }

    private void Update()
    {
        HorizontalFollow();
    }

    private void HorizontalFollow()
    {
        transform.position = new Vector3(
            followTarget.position.x + offsetFromTarget.x, 
            transform.position.y, 
            transform.position.z);
    }
}