using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Enemy
{
    [SerializeField] private float windUpTime;
    [SerializeField] private float chargeTime;
    [SerializeField] private float selfStunTime;
    [SerializeField] private float chargeSpeed;

    [SerializeField] private LayerMask enemy;

    private RaycastHit chargerHit;
    private Rigidbody rb;


    private bool charging = false;
    private bool attacking = false;
    private bool hitPlayer = false;

    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
        base.Start();
    }

    private void FixedUpdate()
    {
        if (charging)
        {
            transform.position += transform.forward * Time.deltaTime * chargeSpeed;//not too bad, still goes through wall if close          
        }
    }

    public void CheckAttack()
    {
        //Needs to constantly face player except when charging
        //spherecast forward and see if it can hit the player in a straight line

        Vector3 centre = new Vector3(transform.position.x, transform.position.y - 2, transform.position.z);
        Vector3 left = new Vector3(transform.position.x - 2, transform.position.y - 2, transform.position.z);
        Vector3 right = new Vector3(transform.position.x + 2, transform.position.y - 2, transform.position.z);

        Debug.DrawRay(centre, transform.TransformDirection(Vector3.forward) * 20, Color.red, 1);
        Debug.DrawRay(left, transform.TransformDirection(Vector3.forward) * 20, Color.red, 1);
        Debug.DrawRay(right, transform.TransformDirection(Vector3.forward) * 20, Color.red, 1);

        if (Physics.SphereCast(centre, 2, transform.forward, out chargerHit, 20, ~enemy) 
            && Physics.SphereCast(left, 2, transform.forward, out chargerHit, 20, ~enemy) 
            && Physics.SphereCast(right, 2, transform.forward, out chargerHit, 20, ~enemy))
        {
            if (chargerHit.collider.GetComponent<Player>())
            {
                //Debug.Log("Detect Player");
                if (!attacking)
                {
                    StartCoroutine(Attack());
                }              
            }
            else
            {
                //Debug.Log("Detect Object Not player");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.GetComponent<Entity>())
        {
            charging = false;
            //Debug.Log(collision); 
        }      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Entity>())
        {
            var player = other.GetComponent<Player>();
            if (player && !hitPlayer && attacking)
            {
                player.TakeDamage(damage);
                other.GetComponent<Rover.Basic.Rover_Controller>().TakeCharge(transform.forward * 250000);
                hitPlayer = true;
            }
        }
        else
        {
            charging = false;
            //Debug.Log(other);
        }
    }

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

                CheckAttack();
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    public override IEnumerator Attack()//charge forward in a straight line at the most recent player position dealing damage along the way
    {
        attacking = true;//ok not amazing
        pathfinder.enabled = false;
        //play roar/starting charge animation
        yield return new WaitForSeconds(windUpTime);

        charging = true;
        //play charge animation
        yield return new WaitForSeconds(chargeTime);
        charging = false;
        rb.isKinematic = true;
         
        yield return new WaitForSeconds(selfStunTime);
        rb.isKinematic = false;
        pathfinder.enabled = true;
        attacking = false;
        hitPlayer = false;
    }
}
