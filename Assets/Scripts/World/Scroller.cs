using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    public Transform player;
    public Transform chunkA;
    public Transform chunkB;

    private float playerWidth;
    private float playerHeight;
    private float chunkWidth;
    private float chunkHeight;
    private BoxCollider2D coll;

    private void Awake()
    {
        Debug.Assert(chunkA != null && chunkB != null);

        coll = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        SpriteRenderer playerRend = 
            player.Find("Torso").GetComponent<SpriteRenderer>();
        playerWidth = playerRend.sprite.bounds.size.x; 
        playerHeight = playerRend.sprite.bounds.size.y;
        SpriteRenderer chunkRend = chunkA.GetComponent<SpriteRenderer>();
        chunkWidth = chunkRend.sprite.bounds.size.x;
        chunkHeight = chunkRend.sprite.bounds.size.y;

        //move chunks in line with player
        Vector3 playerBottomLeft = player.position - 
            new Vector3(playerWidth/2, playerHeight/2);
        Vector3 chunkBottomLeft = chunkA.position -
            new Vector3(chunkWidth/2, chunkHeight/2);
        Vector3 move = playerBottomLeft - chunkBottomLeft;
        chunkA.position += move;
        chunkB.position = chunkA.position + Vector3.right * chunkWidth;

        //setup scroll collider
        float collDistFromEdge = 10f;
        transform.position = 
            Camera.main.ScreenToWorldPoint(
                new Vector3(0, Screen.height/2)) +
            (Vector3.left * (coll.size.x + collDistFromEdge));
        coll.size = new Vector2(coll.size.x, Camera.main.orthographicSize*2);
    }

    /// <summary> 
    /// Moves left most chunk to the right side of other one.
    /// </summary>
    private void Scroll()
    {
        Transform scrollChunk = 
            (chunkA.transform.position.x < chunkB.transform.position.x) ?
            chunkA :
            chunkB; 
        scrollChunk.position += Vector3.right * chunkWidth * 2;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string layer = LayerMask.LayerToName(other.gameObject.layer);
        switch(layer)
        {
            case "Chunk":
                Scroll();
                break;
            default:
                Debug.LogWarning($"Scroller hit unknown object named {other.gameObject.name} on layer {layer}.");
                break;
        }
    }
}
