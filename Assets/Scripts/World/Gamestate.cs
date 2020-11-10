using System;
using System.Collections.Generic;
using UnityEngine;

public class Gamestate : MonoBehaviour
{
    //reference for all enemy types
    public Heresy heresy;

    //determines the max power level that enemies can sum to
    public int difficulty {get; private set;}

    //current enemy numbers, not ceilings
    private int totalEnemies;
    private int totalPowerLevel;

    //util
    public static Gamestate instance;
    private Krieger player;
    private float lastSpawnTime;
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
        lastSpawnTime = -1f;
        enemyPool = new List<Enemy>();
    }

    private void Update()
    {
        //test
        if(Input.GetKeyDown(KeyCode.T))
        {
            Enemy enemy = SpawnEnemy(EnemyType.CULTIST);
            if(enemy != null)
                enemy.transform.position = player.transform.position + new Vector3(100, 0, 0);
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