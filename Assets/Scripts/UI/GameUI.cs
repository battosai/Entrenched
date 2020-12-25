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
    private readonly float textWidthRatio = 7f/1136f;

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
        yield return StartCoroutine(
            Utils.Fade(
                fader,
                Color.clear,
                Color.black,
                1f));
        yield return StartCoroutine(
            Utils.Fade(
                endGameText,
                Color.clear,
                Color.white,
                1f));
    }
}