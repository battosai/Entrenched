using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utils
{
    public static Dictionary<string, string> antiLawsuit = new Dictionary<string, string>()
    {
        {"Blaster", "Lasgun"},
        {"Heatblaster", "Hellgun"},
        {"Shotgun", "Shotgun"},
        {"Protogun", "Plasmagun"},
        {"Fusiongun", "Meltagun"},

        {"Shovel", "Shovel"},
        {"Cutlass", "Powersword"},
        {"Ripper", "Chainsword"},
        {"Gauntlet", "Powerfist"}
    };

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