using System;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    //reference for all enemy types
    public Heresy heresy;


    //determines the max power level that enemies can sum to
    public int difficulty {get; private set;}

    //current enemy numbers, not ceilings
    private int totalEnemies;
    private int totalPowerLevel;

    //util
    public static GameState instance;
    public float spawnInterval;
    private float lastSpawnTime;
    private Krieger player;
    //TODO:
    //make enemyPools into multiple lists based on power level
    //NEED: do it:^) and add more enemies bitch
    private List<Enemy> enemyPool;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else if(instance != this)
            Destroy(this.gameObject);
        
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
        enemyPool = new List<Enemy>();
    }

    private void Update()
    {
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
                //NEED: need more enemies and the enemyPool TODO to be finished

                for(int i = 0; i < powerLevelDeficit; i++)
                {
                    Enemy enemy = SpawnEnemy(EnemyType.CULTIST);
                    if(enemy != null)
                        enemy.transform.position = 
                            player.transform.position + 
                            new Vector3(100 + spacing, 0, 0);

                    //this new spacing will be used for the next enemy
                    spacing = enemy.rend.sprite.bounds.size.x;
                }

                lastSpawnTime = Time.time;
            }
        }
    }

    /// <summary>
    /// Create an enemy from the object pool. Does not position.
    /// </summary>
    private Enemy SpawnEnemy(EnemyType eType)
    {
        if(totalPowerLevel > difficulty)
            return null;

        Enemy enemy = heresy.enemyByType[eType];

        lastSpawnTime = Time.time;
        totalEnemies++;
        totalPowerLevel += enemy.powerLevel;

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

        Enemy newEnemy = Instantiate(heresy.enemyByType[enemy.type]);
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    /// <summary>
    /// Updates enemy numbers with defeated enemy. Called from Enemy.EndDeath animation event.
    /// </summary>
    public static void EnemyDefeated(Enemy enemy)
    {
        instance.difficulty++;
        instance.totalEnemies--;
        instance.totalPowerLevel -= enemy.powerLevel;
    }
}