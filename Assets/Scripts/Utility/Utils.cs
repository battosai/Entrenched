using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utils
{
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
    }
}