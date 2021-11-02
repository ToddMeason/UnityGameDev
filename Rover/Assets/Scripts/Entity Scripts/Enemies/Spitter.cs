using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spitter : Enemy
{
    [SerializeField] private float attackCoolDown;
    [SerializeField] private float attackDelay;
    [SerializeField] private float projectileSpeed;

    private bool attacking = false;
    private RaycastHit rayHit;

    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private Projectile projectile;

    public void CheckAttack()
    {
        //Needs to constantly face player except when charging
        //spherecast forward and see if it can hit the player in a straight line

        Vector3 centre = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);
        Vector3 left = new Vector3(transform.position.x - 2, transform.position.y - 1.5f, transform.position.z);
        Vector3 right = new Vector3(transform.position.x + 2, transform.position.y - 1.5f, transform.position.z);

        Debug.DrawRay(centre, transform.TransformDirection(Vector3.forward) * 20, Color.red, 1);
        Debug.DrawRay(left, transform.TransformDirection(Vector3.forward) * 20, Color.red, 1);//change to display forward properly
        Debug.DrawRay(right, transform.TransformDirection(Vector3.forward) * 20, Color.red, 1);//change to display forward properly

        if (Physics.SphereCast(centre, 2, transform.forward, out rayHit, 20)
            && Physics.SphereCast(left, 2, transform.forward, out rayHit, 20)
            && Physics.SphereCast(right, 2, transform.forward, out rayHit, 20))
        {
            if (rayHit.collider.GetComponent<Player>())
            {
                Debug.Log("Detect Player");
                if (!attacking)
                {
                    StartCoroutine(Attack());
                }
            }
            else
            {
                Debug.Log("Detect Object Not player");
            }
        }
    }

    #region CoRoutines

    public override IEnumerator Attack()//calculate distance to position where attack was called then lob a physics object at that area and it will blow up and leave acid on the ground
    {
        attacking = true;
        pathfinder.enabled = false;

        yield return new WaitForSeconds(attackDelay);
        //play spit animation
        //spawn spit projectile
        Quaternion newRotation = projectileSpawn.rotation;

        newRotation = Quaternion.Euler(projectileSpawn.eulerAngles.x, projectileSpawn.eulerAngles.y, projectileSpawn.eulerAngles.z);
        Projectile newProjectile = Instantiate(projectile, projectileSpawn.position, newRotation) as Projectile;
        newProjectile.SetSpeed(projectileSpeed);
        newProjectile.SetDamage(damage);

        pathfinder.enabled = true;

        yield return new WaitForSeconds(attackCoolDown);
        attacking = false;
    }

    public override IEnumerator UpdatePath()//spitter should try to stay out of range of player
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

                CheckAttack();
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    #endregion
}
