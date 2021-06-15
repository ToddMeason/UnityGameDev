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
    private Transform target;
    private Entity targetEntity;

    private Material skinMaterial;
    private Color originalColor;

    public float expOnDeath;//change to private later
    public float attackDistanceThreshold = 3f;//change to private later
    public float timeBetweenAttacks = 2f;//change to private later
    public float damage = 10;//change to private later

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
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;

        if (GameObject.FindGameObjectWithTag("Player") != null)//Check if there is actually a player
        {
            hasTarget = true;
            currentState = State.Chasing;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            targetEntity = target.GetComponent<Entity>();//Get OnDeath for target to see if it is dead and to stop chasing/attacking it
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;//Need to figure out a way to get box collider edge or just add a capsule collider for a radius float

            StartCoroutine("UpdatePath");
        }
    }

    private void Update()
    {
        if (hit)//Dont know if this works properly
        {
            if (health <= 0) {
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
                    StartCoroutine("Attack");
                }
            }
        }
    }

    #endregion

    #region Custom Methods
    protected override void Die()
    {
        currentState = State.Die;
        player.AddExp(expOnDeath);
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
                animator.Play("Idle_1");
                break;

            case State.Chasing:
                animator.Play("Walk_1");
                break;

            case State.Attacking:
                animator.Play("Attack_1");
                break;

            case State.TakeDamage:
                StartCoroutine(TakeDamageAnimation());
                break;
            
            case State.Die:
                animator.Play("Die");
                break;
        }
        yield return null;
    }

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
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator DieAnimation() {
        pathfinder.enabled = false;
        hasTarget = false;
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

        skinMaterial.color = Color.red;
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

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    #endregion
}
