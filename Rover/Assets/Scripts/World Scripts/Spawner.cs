using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    #region Variables
    public Wave[] waves;
    public Enemy enemy;
    public List<GameObject> pickUps = new List<GameObject>();

    Entity playerEntity;
    Transform playerT;

    private Wave currentWave;
    private int currentWaveNumber;

    private int pickUpsRemainingToSpawn;
    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;//Make this visible on UI later
    private float nextSpawnTime;

    MapGenerator map;

    float timeBetweenCampingChecks = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    bool isDisabled;

    #endregion

    #region Events
    public event System.Action<int> OnNewWave;

    #endregion

    #region Custom Classes
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public int pickUpCount;
        public float timeBetweenSpawns;
    }

    #endregion

    #region Builtin Methods
    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    private void Update()
    {
        if (!isDisabled)
        {
            if(pickUpsRemainingToSpawn > 0)//currently loops through whole list 
            {
                pickUpsRemainingToSpawn--;
                StartCoroutine(SpawnPickUp());
            }

            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);//Checking distance between the two points of current pos and the pos the player was however many secs ago(timeBetweenCampingChecks)
                campPositionOld = playerT.position;
            }

            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)//Spawns enemies for the given wave
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine("SpawnEnemy");
            }
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

    private void OnPlayerDeath()
    {
        isDisabled = true;
    }

    private void ResetPlayerPosition()//resets player back to the centre of the map if using new map gen for each wave
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    private void NextWave()
    {
        currentWaveNumber++;

        if (currentWaveNumber - 1 < waves.Length)//Check if within wave array
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            pickUpsRemainingToSpawn = currentWave.pickUpCount;//sets pickups at start of wave before spawning them

            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();//turn off if not changing map with each wave
        }
    }

    #endregion

    #region Coroutines
    IEnumerator SpawnEnemy()//Gets a random open tile and spawns an enemy there
    {
        float spawnDelay = 1.5f;
        float tileFlashSpeed = 4;

        Transform SpawnTile = map.GetRandomOpenTile();
        if(isCamping)
        {
            SpawnTile = map.GetTileFromPosition(playerT.position);
        }

        Material tileMat = SpawnTile.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;
        Color flashColor = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, SpawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;//Gets the event call from the enemy/entity script that died
    }

    IEnumerator SpawnPickUp()//Change to spawn the selected amount of items but randomlly select which ones spawn 
    {
        for (int i = 0; i < pickUps.Count; i++)
        {
            Transform SpawnTile = map.GetRandomOpenTile();

            Instantiate(pickUps[i], SpawnTile.position + Vector3.up, Quaternion.identity);
        }
        yield return null;
    }

    #endregion
}
