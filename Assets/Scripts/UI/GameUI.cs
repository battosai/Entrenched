using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fader;

    private void Awake()
    {
        Debug.Assert(fader != null);
    }

    private void Start()
    {
        Krieger.OnDeath += FadeWrapper;
    }

    /// <summary>
    /// Fader screen to black. Wrapper subscribes to Krieger OnDeath event.
    /// </summary>
    private void FadeWrapper() {StartCoroutine(Fade());}
    private IEnumerator Fade()
    {
        float fadeTime = 2f;
        float timer = Time.time;
        Color c = Color.clear;
        while(Time.time - timer < fadeTime)
        {
            float elapsedTime = Time.time - timer;
            float alpha = elapsedTime/fadeTime;
            c.a = alpha;
            fader.color = c;
            yield return null;
        }
        fader.color = Color.black;
    }
}