using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int powerLevel;
    public int wounds;
    public int toughness;

    private Rigidbody2D rb;
    private Collider2D coll;

    public delegate void Wounded(int dmg);
    public static Wounded OnWounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    private void Start()
    {
        OnWounded += TakeDamage;
    }

    private void TakeDamage(int dmg)
    {
        wounds = Mathf.Max(0, wounds-dmg);
        //set an anim param for taking a hit
    }
}