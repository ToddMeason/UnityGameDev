using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyTest : Entity
{
    #region Variables
    public enum State { Idle, Chasing, Attacking, TakeDamage, Die }
    [SerializeField] State currentState;

    public Animator animator;

    private NavMeshAgent pathfinder;
    private Player player;
    private Transform target;
    private Entity targetEntity;

    [SerializeField] private float currencyOnDeath;
    [SerializeField] private float expOnDeath;
    [SerializeField] private float attackDistanceThreshold = 3f;
    [SerializeField] private float timeBetweenAttacks = 2f;
    [SerializeField] private float damage = 10;
    private float nextAttackTime;
    private float myCollisionRadius;
    private float targetCollisionRadius;

    private bool hasTarget;

    [SerializeField] private Animation idle;
    [SerializeField] private Animation walk;
    [SerializeField] private Animation attack;
    [SerializeField] private Animation takeDamage;
    [SerializeField] private Animation die;

    #endregion

    #region Builtin Methods
    protected override void Start()
    {
        base.Start();

        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)//Check if there is actually a player
        {
            hasTarget = true;
            currentState = State.Chasing;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            targetEntity = target.GetComponent<Entity>();//Get OnDeath for target to see if it is dead and to stop chasing/attacking it
            targetEntity.OnDeath += OnTargetDeath;//Need to change to target later if enemies have different targets

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;//Need to figure out a way to get box collider edge or just add a capsule collider for a radius float

            StartCoroutine(UpdatePath());
        }
    }

    private void Update()
    {
        if (hit)//Dont know if this works properly
        {
            if (health <= 0)
            {
                currentState = State.Die;
            }
            else
            {
                currentState = State.TakeDamage;
            }
        }
        else
        {
            currentState = State.Chasing;
        }

        StartCoroutine(PlayAnimationState());

        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDisToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDisToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    Attack();
                }
            }
        }
    }

    #endregion

    #region Custom Methods
    public abstract void Attack();

    protected override void Die()//change this as it does not play properly
    {
        currentState = State.Die;
        dead = true;
        player.AddExp(expOnDeath);
        player.AddCurrency(currencyOnDeath);
        StartCoroutine(DieAnimation());
    }

    private void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    #endregion

    #region Coroutines
    IEnumerator PlayAnimationState()
    {
        switch (currentState)
        {
            case State.Idle:
                idle.Play();
                break;

            case State.Chasing:
                walk.Play();
                break;

            case State.Attacking:
                attack.Play();
                break;

            case State.TakeDamage:
                StartCoroutine(TakeDamageAnimation());
                break;

            case State.Die:
                die.Play();
                break;
        }
        yield return null;
    }

    IEnumerator DieAnimation()
    {
        pathfinder.enabled = false;
        hasTarget = false;
        die.Play();
        yield return new WaitForSeconds(2f);
        base.Die();
    }

    IEnumerator TakeDamageAnimation()
    {
        pathfinder.enabled = false;
        takeDamage.Play();
        yield return new WaitForSeconds(0.5f);
        hit = false;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {

        float refreshRate = 0.25f;//Runs the pathfinder based on the refreshrate variable instead of each frame (for performance)

        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);//Makes it so the movement only goes just into the target capsule collider instead of directly to the middle of their position
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    #endregion
}
