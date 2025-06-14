// Standard library includes
using System;
using System.Collections;
using System.Collections.Generic;

// Unity includes
using Random=UnityEngine.Random;
using UnityEngine;

/// <summary>
/// Manager for game loop.
/// </summary>
public class GameState : MonoBehaviour
{
    [Header("Test")]
    public bool testMode;

    [Header("Development")]
    //reference for game ui
    public GameUI ui;

    //reference for all enemy types
    public Heresy heresy;

    //reference for ammo prefab
    public Ammo ammoDrop;
    public List<Ammo> ammoPool {get; private set;}

    //reference for projectile prefab
    public Projectile projectile;
    public List<Projectile> projectilePool {get; private set;}

    //determines the max power level that enemies can sum to
    public int difficulty {get; private set;}

    //current enemy numbers, not ceilings
    private int totalEnemies;
    private int totalPowerLevel;

    /// <summary>
    /// Event for when spawner needs to take a break.
    /// </summary>
    public delegate void SpawnerPause(float duration_s);
    public SpawnerPause OnSpawnerPause;

    //util
    public static GameState instance;
    public bool isReady; //only true once player has selected weapons
    public float spawnInterval_s;
    private float lastSpawnTime_s;
    private Krieger player;
    private Dictionary<int, List<Enemy>> enemyPoolsByPowerLevel;

    //stat tracker
    public int enemiesDefeated {get; private set;}
    public int feetTraversed {get; private set;}

    private void Awake()
    {
        //always replace instance bc we know
        //there will only be one instance active at a time
        instance = this;

        Debug.Assert(ui != null);
        Debug.Assert(heresy != null);

        heresy.Define();

        player = GameObject.FindWithTag("Player").GetComponent<Krieger>();
    }

    private void Start()
    {
        difficulty = 1;
        totalPowerLevel = 0;
        totalEnemies = 0;
        lastSpawnTime_s = -1f;
        enemyPoolsByPowerLevel = new Dictionary<int, List<Enemy>>()
        {
            {1, new List<Enemy>()},
            {2, new List<Enemy>()},
            {3, new List<Enemy>()}
        };
        ammoPool = new List<Ammo>();
        projectilePool = new List<Projectile>();

        OnSpawnerPause += PauseSpawnerWrapper;
        Krieger.instance.OnDeath += SaveDistanceTraversed;
    }

    /// <summary>
    /// Cleanup.
    /// </summary>
    private void OnDestroy()
    {
        OnSpawnerPause -= PauseSpawnerWrapper;
        Krieger.instance.OnDeath -= SaveDistanceTraversed;
    }

    private void Update()
    {
        if(Krieger.instance.isDead)
            return;

        if(!isReady)
            return;

        Spawner();
    }

    /// <summary>
    /// Handles spawning enemies as difficulty increases etc.
    /// </summary>
    private void Spawner()
    {
        // // This was for a spawner based on a timer
        // // Swapped out for a wave-based spawner
        // if(Time.time - lastSpawnTime_s <= spawnInterval_s)
        // {
        //     return;
        // }
        // // Chance to not spawn an enemy this interval
        // float roll = UnityEngine.Random.value;
        // if (roll < 0.05)
        // {
        //     lastSpawnTime_s = Time.time;
        //     return;
        // }

        // Opted to base spawner on "waves",
        // so we only spawn once all enemies are dead.
        if (totalPowerLevel > 0)
        {
            return;
        }

        float spacing = 0;
        int powerLevelDeficit = difficulty - totalPowerLevel;

        if(powerLevelDeficit > 0)
        {
            while(powerLevelDeficit > 0)
            {
                //randomly pick a power level to spawn for
                //between 1 and deficit or highest level we have
                int incomingPowerLevel = Random.Range(
                    1, 
                    1 + Math.Min(
                        enemyPoolsByPowerLevel.Count, 
                        powerLevelDeficit));

                powerLevelDeficit -= incomingPowerLevel;

                Enemy enemy = SpawnEnemy(incomingPowerLevel);

                if (enemy != null)
                {
                    enemy.transform.position = player.transform.position + 
                        new Vector3(
                            150 + spacing, 
                            0, 
                            0);
                }

                //this new spacing will be used for the next enemy
                spacing += enemy.rend.sprite.bounds.size.x;
            }

            lastSpawnTime_s = Time.time;
        }
    }

    /// <summary>
    /// Create an enemy of specific type. Does not position.
    /// </summary>
    private Enemy SpawnEnemy(EnemyType eType)
    {
        Enemy enemy = heresy.enemyByType[eType];

        lastSpawnTime_s = Time.time;
        totalEnemies++;
        totalPowerLevel += enemy.powerLevel;

        List<Enemy> enemyPool = enemyPoolsByPowerLevel[enemy.powerLevel];
        foreach(Enemy e in enemyPool)
        {
            if(enemy.type == e.type)
            {
                if(!e.gameObject.activeInHierarchy)
                {
                    e.gameObject.SetActive(true);
                    return e;
                }
            }
        }

        Enemy newEnemy = Instantiate(enemy);
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    /// <summary>
    /// Create an enemy of a specific power level. Does not position.
    /// </summary>
    private Enemy SpawnEnemy(int powerLevel)
    {
        //randomly pick which enemy of that power level to spawn
        List<EnemyType> eTypes = heresy.typesByPowerLevel[powerLevel];
        EnemyType eType = eTypes[
            Random.Range(
                0,
                eTypes.Count)];
        Enemy enemy = heresy.enemyByType[eType];

        lastSpawnTime_s = Time.time;
        totalEnemies++;
        totalPowerLevel += enemy.powerLevel;

        List<Enemy> enemyPool = enemyPoolsByPowerLevel[powerLevel];
        foreach(Enemy e in enemyPool)
        {
            if(enemy.type == e.type)
            {
                if(!e.gameObject.activeInHierarchy)
                {
                    e.gameObject.SetActive(true);
                    return e;
                }
            }
        }

        Enemy newEnemy = Instantiate(enemy);
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    /// <summary>
    /// Wrapper for pausing spawner coroutine.
    /// </summary>
    private void PauseSpawnerWrapper(float duration_s)
    {
        StartCoroutine(PauseSpawner(duration_s));
    }

    /// <summary>
    /// Coroutine that pauses the spawner for a designated amount of time.
    /// </summary>
    private IEnumerator PauseSpawner(float duration_s)
    {
        float time_s = Time.time;
        float elapsedSpawnInterval_s = time_s - lastSpawnTime_s;

        while (Time.time - time_s < duration_s)
        {
            lastSpawnTime_s = Time.time - elapsedSpawnInterval_s;
            yield return null;
        }
    }

    /// <summary>
    /// Updates enemy numbers with defeated enemy. Called from Enemy.EndDeath animation event.
    /// </summary>
    public static void EnemyDefeated(Enemy enemy)
    {
        instance.difficulty++;
        instance.totalEnemies--;
        instance.totalPowerLevel -= enemy.powerLevel;

        instance.enemiesDefeated++;
    }

    /// <summary>
    /// Subscriber to Krieger.OnDeath Event. Find end game distance stat.
    /// </summary>
    private void SaveDistanceTraversed()
    {
        int dist = Utils.GetDistanceTraversed();
        feetTraversed = dist;

        // Save if new record
        float playerDist = PlayerPrefs.GetFloat("Distance", 0);
        if(dist > playerDist)
        {
            PlayerPrefs.SetFloat("Distance", dist);
            PlayerPrefs.Save();
        }
    }
}