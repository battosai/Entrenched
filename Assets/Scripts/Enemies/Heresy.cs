using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Enemies/Heresy"))]
public class Heresy : ScriptableObject
{
    public List<Enemy> enemies;

    public Dictionary<EnemyType, Enemy> enemyByType;
    public Dictionary<int, List<EnemyType>> typeByPowerLevel;

    /// <summary>
    /// Creates dictionaries by powerLevel, Enemy type, etc.
    /// </summary>
    public void Define()
    {
        enemyByType = new Dictionary<EnemyType, Enemy>();
        typeByPowerLevel = new Dictionary<int, List<EnemyType>>();
        foreach(Enemy e in enemies)
        {
            enemyByType.Add(e.type, e);

            if(typeByPowerLevel.ContainsKey(e.powerLevel))
                typeByPowerLevel[e.powerLevel].Add(e.type);
            else
                typeByPowerLevel.Add(e.powerLevel, new List<EnemyType>{e.type});
        }
    }
}