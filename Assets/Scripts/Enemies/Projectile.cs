using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb {get; private set;}
    public Animator anim {get; private set;}
    public AudioClip[] impacts;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        Krieger.instance.OnDeath += Freeze;
    }

    /// <summary>
    /// Cleanup.
    /// </summary>
    private void OnDestroy()
    {
        Krieger.instance.OnDeath -= Freeze;
    }

    /// <summary>
    /// Spawn projectile at position with velocity.
    /// </summary>
    public static void Spawn(
        Vector2 position,
        float speed)
    {
        List<Projectile> pool = GameState.instance.projectilePool;
        Projectile projectile = null;

        foreach(Projectile p in pool)
        {
            if(!p.gameObject.activeInHierarchy)
            {
                projectile = p;
                projectile.anim.SetBool("Hit", false);
                projectile.gameObject.SetActive(true);
                break;
            }
        }

        if(projectile == null)
        {
            projectile = Instantiate(GameState.instance.projectile);
            pool.Add(projectile);
        }

        projectile.transform.position = position;
        projectile.rb.velocity = Vector2.left * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null ||
            other.tag == "Enemies")
        {
            return;
        }

        rb.velocity = Vector2.zero;

        if(other.tag == "Player")
        {
            anim.SetBool("Hit", true);
            Krieger.instance.OnWounded?.Invoke();
            //use krieger audio source for now
            AudioManager.PlayOneClip(
                Krieger.instance.audioSource,
                impacts);
        }
        else
            this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Animation Event: Deactivate object after collision w/ Scroller or Player.
    /// </summary>
    private void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Subscriber to Krieger OnDeath event. Freeze projectile.
    /// </summary>
    private void Freeze()
    {
        anim.speed = 0f;
        rb.velocity = Vector3.zero;
    }
}