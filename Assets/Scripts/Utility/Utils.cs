using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    /// <summary>
    /// Resizes text object by scaling to a predetermined hardcoded ratio.
    /// </summary>
    public static void AdjustTextScaleForCustomFont(Text text)
    {
        // Custom font needs to scale via transform
        // Ratio established w/ iPhone 5/5S/5C/SE
        // (scale of 7)/(viewport width AKA Camera.pixelWidth of 1136)
        float textWidthRatio = 7f/1136f;
        text.transform.localScale = Vector3.one * Camera.main.pixelWidth * textWidthRatio;
    }

    /// <summary>
    /// Fits knob image to slider image based on our sprites.
    /// Coroutine to delay by 1 frame so when called at Start we get accurate values.
    /// </summary>
    public static IEnumerator FitKnobToSlider(Slider slider)
    {
        yield return null;

        Image sliderBarImg = slider.transform.Find("Background").GetComponent<Image>();
        Image sliderKnobImg = slider.transform.Find("Handle Slide Area/Handle").GetComponent<Image>();
        RectTransform sliderTrans = sliderBarImg.GetComponent<RectTransform>();
        RectTransform knobTrans = sliderKnobImg.GetComponent<RectTransform>();

        Vector2 sliderBarToKnobRatio = 
            new Vector2(
                sliderBarImg.sprite.bounds.size.x / sliderKnobImg.sprite.bounds.size.x,
                sliderBarImg.sprite.bounds.size.y / sliderKnobImg.sprite.bounds.size.y);

        knobTrans.sizeDelta = 
            new Vector2(
                sliderTrans.rect.width / sliderBarToKnobRatio.x, 
                sliderTrans.rect.height / sliderBarToKnobRatio.y);
    }

    /// <summary>
    /// "Bi-directional" dictionary for WH40K terms
    /// </summary>
    public static Dictionary<string, string> antiLawsuit = new Dictionary<string, string>()
    {
        {"Blaster", "Lasgun"}, {"Lasgun", "Blaster"},
        {"Heatblaster", "Hellgun"}, {"Hellgun", "Heatblaster"},
        {"Shotgun", "Shotgun"},
        {"Protogun", "Plasmagun"}, {"Plasmagun", "Protogun"},
        {"Fusiongun", "Meltagun"}, {"Meltagun", "Fusiongun"},

        {"Shovel", "Shovel"},
        {"Cutlass", "Powersword"}, {"Powersword", "Cutlass"},
        {"Ripper", "Chainsword"}, {"Chainsword", "Ripper"},
        {"Gauntlet", "Powerfist"}, {"Powerfist", "Gauntlet"}
    };

    /// <summary>
    /// Fade any object that has a Color property.
    /// </summary>
    public static IEnumerator Fade(
        object element,
        Color start,
        Color end,
        float duration)
    {
        float timer = Time.time;
        Color diff = end - start;
        PropertyInfo colorProperty = element.GetType().GetProperty("color");
        while(Time.time - timer < duration)
        {
            float elapsedTime = Time.time - timer;
            float ratio = elapsedTime/duration;
            Color current = start + (diff * ratio);
            colorProperty.SetValue(element, current);
            yield return null;
        }
        colorProperty.SetValue(element, end);
    }
}