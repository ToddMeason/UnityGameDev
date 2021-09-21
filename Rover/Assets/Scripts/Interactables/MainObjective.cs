using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainObjective : Interactable //Make spawning its own class later
{
    public Enemy[] enemies;
    public LayerMask terrainLayer;//May add this to sphereCast later if i change the ground layer

    [SerializeField] private float maxSpawnPos = 20;

    public float time;
    public float spawnDelay = 2f;
    public bool playerInRange;

    private float nextSpawnTime;
    private bool isDisabled = false;

    [SerializeField]private GameObject dropShip;

    private void Update()
    {
        
    }

    public override void Interact()
    {
        if (!activated)
        {
            StartCoroutine(Timer());          

            activated = true;
        }

        base.Interact();
    }

    public void GetClearArea()//May change ground tiles to own ground layer and check everything but ground layer
    {
        //Get random coord in area on flat axis, spherecast down from the sky onto that coord, if no colliders besides ground if there than spawn the enemy if not get a new coord
        while (!isDisabled)
        {
            Vector3 pos = new Vector3(transform.position.x + Random.Range(-maxSpawnPos, maxSpawnPos), 0.5f, transform.position.z + Random.Range(-maxSpawnPos, maxSpawnPos));//might need to change to z not y
            Debug.DrawLine(transform.position, pos, Color.red, 5);
            if (Physics.Raycast(pos, Vector3.up, 10))
            {
                SpawnEnemies(pos);
                //Debug.Log("Found Pos" + pos);
                return;
            }
            Debug.Log("didn't find Pos");
        }             
    }

    public void SpawnEnemies(Vector3 spawnPos)
    {
        if (!isDisabled)
        {
            Enemy spawnedEnemy = Instantiate(enemies[Random.Range(0, enemies.Length)] , spawnPos + Vector3.up, Quaternion.identity) as Enemy;
            spawnedEnemy.transform.parent = this.transform;
        }
    }

    private void OnPlayerDeath()
    {
        isDisabled = true;
    }

    private void OnObjectiveComplete()
    {
        StopAllCoroutines();
    }

    private void OnEnable()
    {
        FindObjectOfType<Player>().GetComponent<Player>().OnDeath += OnPlayerDeath;      
    }

    #region Coroutines
    public IEnumerator Timer()
    {
        float curTime = 0;       
        float spawnTime = 0;

        while (playerInRange || isDisabled)
        {
            curTime += Time.deltaTime;
            //Debug.Log(curTime);//Add to UI later
            isDisabled = false;

            spawnTime += Time.deltaTime;
            if (spawnTime >= spawnDelay)
            {
                GetClearArea();
                spawnTime = 0;
            }

            if (curTime >= time)
            {
                isDisabled = true;
                Instantiate(dropShip, transform.position + new Vector3(0, 15, 0), Quaternion.identity);
                OnObjectiveComplete();
                yield return null;
            }
            yield return null;
        }       
    }
    #endregion

}
