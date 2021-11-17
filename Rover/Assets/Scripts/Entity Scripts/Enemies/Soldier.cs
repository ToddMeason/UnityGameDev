using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Enemy
{
    public override IEnumerator UpdatePath()
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

    public override IEnumerator Attack()
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
            if (percent >= 0.5f && !hasAppliedDamage)
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
 
}
