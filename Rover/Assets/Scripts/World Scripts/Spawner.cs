using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour //rework to not spawn when objective is spawning and to not reset on new wave
{
    #region Variables
    public Wave[] waves;
    public Enemy[] enemy;
    public List<GameObject> chests = new List<GameObject>();
    public MainObjective mainObjective;

    Entity playerEntity;
    Transform playerT;

    private Wave currentWave;
    private int currentWaveNumber;

    private int chestsRemainingToSpawn;
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
        public int chestCount;
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

        StartCoroutine(SpawnMainObjective());
    }

    private void Update()
    {
        if (!isDisabled)
        {
            if (chestsRemainingToSpawn > 0)//currently loops through whole list 
            {
                chestsRemainingToSpawn--;
                StartCoroutine(SpawnChest());
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

                StartCoroutine(SpawnEnemy());
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

            chestsRemainingToSpawn = currentWave.chestCount;//sets pickups at start of wave before spawning them

            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            //ResetPlayerPosition();//turn off if not changing map with each wave
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

        int x;
        if (Random.Range(0, 100) > 90)
        {
            x = 1;
        }
        else
        {
            x = 0;
        }

        Enemy spawnedEnemy = Instantiate(enemy[x], SpawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.transform.parent = this.transform;
        //spawnedEnemy.OnDeath += OnEnemyDeath;//Gets the event call from the enemy/entity script that died
    }

    IEnumerator SpawnChest()//Change to spawn chests properly later not from a list
    {
        for (int i = 0; i < chests.Count; i++)
        {
            Transform SpawnTile = map.GetRandomOpenTile();

            var chest = Instantiate(chests[i], SpawnTile.position + Vector3.up - new Vector3(0,0.5f,0), Quaternion.identity);
            chest.transform.parent = this.transform;
        }
        yield return null;
    }

    IEnumerator SpawnMainObjective()
    {
        Transform SpawnTile = map.GetRandomOpenTile();

        var mainObj = Instantiate(mainObjective, SpawnTile.position + Vector3.up + new Vector3(0, 2, 0), Quaternion.identity);//set height later once prefab model added
        mainObj.transform.parent = this.transform;

        yield return null;
    }

    #endregion
}
