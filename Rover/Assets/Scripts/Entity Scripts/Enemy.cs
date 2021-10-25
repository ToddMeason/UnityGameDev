using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : Entity
{
    #region Variables
    public enum State {Idle, Chasing, Attacking, TakeDamage, Die}
    [SerializeField] State currentState;

    public Animator animator;

    private NavMeshAgent pathfinder;
    private Player player;
    [SerializeField] private Transform target;
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
        if (!target)
        {
            currentState = State.Idle;
        }
        if (!dead)
        {
            switch (currentState)
            {
                case State.Idle:
                    animator.Play("Idle_1");
                    StopCoroutine(UpdatePath());
                    pathfinder.enabled = false;
                    break;

                case State.Chasing:
                    animator.Play("Walk_1");
                    StartCoroutine(UpdatePath());
                    break;

                case State.Attacking:
                    animator.Play("Attack_1");
                    break;

                case State.TakeDamage:
                    StartCoroutine(TakeDamageAnimation());
                    break;
            }

            if (hit)//Dont know if this works properly
            {
                if (health > 0)
                    currentState = State.TakeDamage;
            }
            else
            {
                currentState = State.Chasing;
            }
        }
    }

    #endregion

    #region Custom Methods
    protected override void Die()
    {
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

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;//Runs the pathfinder based on the refreshrate variable instead of each frame (for performance)

        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold/2);//Makes it so the movement only goes just into the target capsule collider instead of directly to the middle of their position
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }

                if (Time.time > nextAttackTime)
                {
                    float sqrDisToTarget = (target.position - transform.position).sqrMagnitude;
                    if (sqrDisToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2))
                    {
                        currentState = State.Attacking;
                        nextAttackTime = Time.time + timeBetweenAttacks;
                        StartCoroutine(Attack());
                    }
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator DieAnimation() {
        pathfinder.enabled = false;
        hasTarget = false;
        animator.Play("Die");
        yield return new WaitForSeconds(2f);
        base.Die();
    }

    IEnumerator TakeDamageAnimation() {
        pathfinder.enabled = false;
        animator.Play("Take_Damage_1");
        yield return new WaitForSeconds(0.5f);
        hit = false;
        pathfinder.enabled = true;
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius + (targetCollisionRadius / 2));//Makes it so the attack only goes just into the target capsule collider instead of directly to the middle of their position

        float percent = 0;
        float attackSpeed = 3;

        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if(percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;//Parabola for 0 to 1 then back to 0 ie move enemy from current position to the player position then back
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);//Move the enemy from current position to the player position then back

            yield return null;
        }

        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    #endregion
}
