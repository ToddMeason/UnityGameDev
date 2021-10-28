using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Enemy
{ 
    public override void CheckAttack()
    {
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
