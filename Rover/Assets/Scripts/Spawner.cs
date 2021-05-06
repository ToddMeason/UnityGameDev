using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Variables
    public Wave[] waves;
    public Enemy enemy;

    private Wave currentWave;
    private int currentWaveNumber;

    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;
    private float nextSpawnTime;
    #endregion

    #region Custom Classes
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float timeBetweenSpawns;
    }

    #endregion

    #region Builtin Methods
    private void Start()
    {
        NextWave();
    }

    private void Update()
    {
        if(enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)//Spawns enemies for the given wave
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            spawnedEnemy.OnDeath += OnEnemyDeath;//Gets the event call from the enemy/entity script that died
        }
    }
    #endregion


    #region Custom Methods
    private void OnEnemyDeath()
    {
        //Counts the enemies remaining then starts next wave
        enemiesRemainingAlive--;

        if (enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    private void NextWave()
    {
        currentWaveNumber++;
        print("wave: " + currentWaveNumber);
        if (currentWaveNumber - 1 < waves.Length)//Check if within wave array
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
        }
    }

    #endregion
}
