using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Enemies/Heresy"))]
public class Heresy : ScriptableObject
{
    public List<Enemy> enemies;

    public Dictionary<Enemy.Type, Enemy> enemyByType;
    public Dictionary<int, List<Enemy.Type>> typeByPowerLevel;

    /// <summary>
    /// Creates dictionaries by powerLevel, Enemy type, etc.
    /// </summary>
    public void Define()
    {
        enemyByType = new Dictionary<Enemy.Type, Enemy>();
        typeByPowerLevel = new Dictionary<int, List<Enemy.Type>>();
        foreach(Enemy e in enemies)
        {
            enemyByType.Add(e.type, e);

            if(typeByPowerLevel.ContainsKey(e.powerLevel))
                typeByPowerLevel[e.powerLevel].Add(e.type);
            else
                typeByPowerLevel.Add(e.powerLevel, new List<Enemy.Type>{e.type});
        }
    }
}