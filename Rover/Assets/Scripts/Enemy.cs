using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Entity
{
    #region Variables
    private NavMeshAgent pathfinder;
    private Player player;
    private Transform target;

    public float expOnDeath;
    
    #endregion

    #region Builtin Methods
    public override void Start()
    {
        base.Start();
        
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        StartCoroutine("UpdatePath");
    }

    private void Update()
    {

    }

    #endregion


    #region Custom Methods
    public override void Die()
    {
        Debug.Log(player);
        player.AddExp(expOnDeath);
        base.Die();
    }

    #endregion

    #region Coroutines
    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, 0, target.position.z);
            pathfinder.SetDestination(targetPosition);
            yield return new WaitForSeconds(refreshRate);
        }
    }
    #endregion
}
