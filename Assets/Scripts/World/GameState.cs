using System;
using System.Collections.Generic;
using UnityEngine;

using Random=UnityEngine.Random;

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

    //util
    public static GameState instance;
    public bool isReady; //only true once player has selected weapons
    public float spawnInterval;
    private float lastSpawnTime;
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
        lastSpawnTime = -1f;
        enemyPoolsByPowerLevel = new Dictionary<int, List<Enemy>>()
        {
            {1, new List<Enemy>()},
            {2, new List<Enemy>()}
        };
        ammoPool = new List<Ammo>();
        projectilePool = new List<Projectile>();

        Krieger.instance.OnDeath += CalculateDistanceTraversed;
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
        if(Time.time - lastSpawnTime > spawnInterval)
        {
            float spacing = 0;
            int powerLevelDeficit = difficulty - totalPowerLevel;
            if(powerLevelDeficit > 0)
            {
                //TODO:
                //spawn stronger enemies if there are higher numbers of enemies and enough space. will probably be a sort of rework for this initial system
                //NEED: do it, currently just randomly selects any plausible ones, could lean towards stronger ones

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
                    if(enemy != null)
                        enemy.transform.position = 
                            player.transform.position + 
                            new Vector3(100 + spacing, 0, 0);

                    //this new spacing will be used for the next enemy
                    spacing += enemy.rend.sprite.bounds.size.x;
                }

                lastSpawnTime = Time.time;
            }
        }
    }

    /// <summary>
    /// Create an enemy of specific type. Does not position.
    /// </summary>
    private Enemy SpawnEnemy(EnemyType eType)
    {
        Enemy enemy = heresy.enemyByType[eType];

        lastSpawnTime = Time.time;
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

        lastSpawnTime = Time.time;
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
    /// Updates enemy numbers with defeated enemy. Called from Enemy.EndDeath animation event.
    /// </summary>
    public static void EnemyDefeated(Enemy enemy)
    {
        //TODO:
        //difficulty should probably scale with distance covered rather than enemies defeated
        //NEEDS: do it
        instance.difficulty++;
        instance.totalEnemies--;
        instance.totalPowerLevel -= enemy.powerLevel;

        instance.enemiesDefeated++;
    }

    /// <summary>
    /// Subscriber to Krieger.OnDeath Event. Find end game distance stat.
    /// </summary>
    private void CalculateDistanceTraversed()
    {
        int dist = (int)Mathf.Floor(Krieger.instance.transform.position.x / 3);
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