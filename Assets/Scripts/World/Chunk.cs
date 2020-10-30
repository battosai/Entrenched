using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private BoxCollider2D coll;
    private SpriteRenderer rend;

    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();

        Debug.Assert(coll != null && rend != null);
    }

    private void Start()
    {
        //initialize scroll collider
        float chunkWidth = rend.sprite.bounds.size.x;
        float chunkHeight = rend.sprite.bounds.size.y;
        coll.offset = Vector2.right * (chunkWidth/2 - coll.size.x/2);
    }
}
