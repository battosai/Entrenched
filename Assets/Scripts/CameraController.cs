using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;
    private SpriteRenderer targetRend;
    private float targetWidth;
    private float targetHeight;
    private Vector2 offsetFromTarget;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        targetRend = followTarget.GetComponent<SpriteRenderer>();
        Debug.Assert(targetRend != null);
    }

    private void Start()
    {
        //initially position target at bottom left
        targetWidth = targetRend.sprite.bounds.size.x;
        targetHeight = targetRend.sprite.bounds.size.y;
        Vector3 currentBottomLeft = cam.ScreenToWorldPoint(new Vector3(0f, 0f, 10f));
        currentBottomLeft += new Vector3(targetWidth/2f-1f, targetHeight/2f-1f, 0f);
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
        Debug.Assert(followTarget != null);

        transform.position = new Vector3(
            followTarget.position.x + offsetFromTarget.x, 
            transform.position.y, 
            transform.position.z);
    }
}