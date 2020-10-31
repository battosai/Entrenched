using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Wargear/Armory"))]
public class Armory : ScriptableObject
{
    public List<RangedWeapon> _ranged;
    public Dictionary<string, RangedWeapon> ranged;
    public List<MeleeWeapon> _melee;
    public Dictionary<string, MeleeWeapon> melee;

    /// <summary>
    /// Converts lists into dictionaries since dictionaries can't be serialized into the editor.
    /// </summary>
    public void Define()
    {
        ranged = new Dictionary<string, RangedWeapon>();
        foreach(RangedWeapon wep in _ranged)
            ranged?.Add(wep.name, wep);

        melee = new Dictionary<string, MeleeWeapon>();
        foreach(MeleeWeapon wep in _melee)
            melee?.Add(wep.name, wep);
    }
}