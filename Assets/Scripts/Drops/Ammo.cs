using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public static float dropChance = 0.05f;

    public Animator anim {get; private set;}

    /// <summary>
    /// Spawn ammo at position.
    /// </summary>
    public static void Spawn(Vector3 position)
    {
        List<Ammo> ammoPool = GameState.instance.ammoPool;
        Ammo ammo = null;

        foreach(Ammo a in ammoPool)
        {
            if(!a.gameObject.activeInHierarchy)
            {
                ammo = a;
                ammo.gameObject.SetActive(true);

                //only pooled objs need to set this trigger
                //newly instantiated will enter spawn state by default
                ammo.anim.SetTrigger("Spawn");
                break;
            }
        }

        if(ammo == null)
        {
            ammo = Instantiate(GameState.instance.ammoDrop);
            ammoPool.Add(ammo);
        }

        ammo.transform.position = position;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Krieger.instance.clips = Math.Min(Krieger.instance.maxClips, Krieger.instance.clips+1);
        anim.SetTrigger("Pickup");
        AudioManager.PlayOneClip(Krieger.instance.audioSource, Krieger.instance.ammoPickups);
    }

    /// <summary>
    /// Animation Event: Hides obj after pickup is finished.
    /// </summary>
    public void Cleanup()
    {
        gameObject.SetActive(false);
    }
}