using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class Enemy : Entity
{
    #region Variables
    public enum State {Idle, Chasing, Attacking, TakeDamage, Die}
    public State currentState;

    public Animator animator;

    public NavMeshAgent pathfinder;
    private Player player;
    public Transform target;
    public Entity targetEntity;

    public float currencyOnDeath;
    public float expOnDeath;
    public float attackDistanceThreshold = 3f;
    public float timeBetweenAttacks = 2f;
    public float damage = 10;
    public float nextAttackTime;
    public float myCollisionRadius;
    public float targetCollisionRadius;

    public bool hasTarget;
    public bool active = false;

    [Header("Animations")]//make sure all of these are the exact name of the animation
    public string walkAnimation;
    public string attackAnimation;
    public string takeDamageAnimation;
    public string dieAnimation;

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
                    pathfinder.enabled = false;
                    break;

                case State.Chasing:
                    animator.Play(walkAnimation);//change to generic variable
                    break;

                case State.Attacking://make part of the chasing state. Where the ai stops and plays attack animation and attack method then continues chasing
                    animator.Play(attackAnimation);//change to generic variable
                    break;

                case State.TakeDamage:
                    if (!active)
                    {
                        StartCoroutine(TakeDamageAnimation());
                    }
                    break;
            }

            if (hit)//Dont know if this works properly
            {
                if (health > 0)
                    currentState = State.TakeDamage;
            }
            else
            {
                if (!active)
                {
                    currentState = State.Chasing;
                }               
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

    public abstract IEnumerator UpdatePath();
 

    IEnumerator DieAnimation() {
        pathfinder.enabled = false;
        hasTarget = false;
        animator.Play(dieAnimation);//change to generic variable
        yield return new WaitForSeconds(3f);
        base.Die();
    }

    IEnumerator TakeDamageAnimation() {
        pathfinder.enabled = false;
        animator.Play(takeDamageAnimation);//change to generic variable
        yield return new WaitForSeconds(0.5f);
        hit = false;
        pathfinder.enabled = true;
    }

    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public abstract IEnumerator Attack();

    #endregion
}
