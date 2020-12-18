using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fader;
    public Text endGameText;

    // Custom font needs to scale via transform
    // Ratio established w/ iPhone 5/5S/5C/SE
    // (scale of 7)/(viewport width AKA Camera.pixelWidth of 1136)
    private float textWidthRatio = 7f/1136f;

    private void Awake()
    {
        Debug.Assert(fader != null);
    }

    private void Start()
    {
        endGameText.transform.localScale = Vector3.one * Camera.main.pixelWidth * textWidthRatio;
        Krieger.OnDeath += EndScreenSequenceWrapper;
    }

    /// <summary>
    /// Subscriber to Krieger.OnDeath Event. Starts the end screen sequence.
    /// </summary>
    private void EndScreenSequenceWrapper() {StartCoroutine(EndScreenSequence());}
    private IEnumerator EndScreenSequence() 
    {
        yield return StartCoroutine(FadeScreen(Color.black));
        yield return StartCoroutine(FadeText(1f));
    }

    /// <summary>
    /// Fade end screen text in and out.
    /// </summary>
    private IEnumerator FadeText(float targetAlpha)
    {
        float fadeTime = 1f;
        float timer = Time.time;
        float alphaDiff = targetAlpha - endGameText.color.a;
        Color c = endGameText.color;
        while(Time.time - timer < fadeTime)
        {
            float elapsedTime = Time.time - timer;
            float ratio = elapsedTime/fadeTime;

            //have to do the ratio from targetAlpha as base point
            //bc initial endGameText.color.a will be lost
            c.a = targetAlpha - (alphaDiff * (1-ratio));
            endGameText.color = c;
            yield return null;
        }
        c.a = targetAlpha;
        endGameText.color = c;
    }

    /// <summary>
    /// Fader screen to desired color.
    /// </summary>
    private IEnumerator FadeScreen(Color targetColor)
    {
        float fadeTime = 1f;
        float timer = Time.time;
        Color colorDiff = targetColor - fader.color;
        while(Time.time - timer < fadeTime)
        {
            float elapsedTime = Time.time - timer;
            float ratio = elapsedTime/fadeTime;

            //have to do the ratio from targetColor as base point 
            //bc initial fader.color will be lost
            fader.color = targetColor - (colorDiff * (1-ratio));
            yield return null;
        }
        fader.color = targetColor;
    }
}