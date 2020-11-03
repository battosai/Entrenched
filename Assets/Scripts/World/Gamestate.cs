using System;
using UnityEngine;

public class Gamestate : MonoBehaviour
{
    public int difficulty {get; private set;}

    private int totalEnemies;
    private int totalPowerLevel;

    private float lastSpawnTime;

    private void Start()
    {
        lastSpawnTime = -1;
    }

    /// <summary>
    /// Add an enemy of a specified powerLevel if there's room.
    /// </summary>
    private void SpawnEnemy(int powerLevel)
    {
        if(totalPowerLevel > difficulty)
            return;

        lastSpawnTime = Time.time;
        totalEnemies++;
        totalPowerLevel += powerLevel;
    }
}