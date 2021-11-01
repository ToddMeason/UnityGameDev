using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Enemy
{
    [SerializeField] private float windUpTime;
    [SerializeField] private float chargeTime;
    [SerializeField] private float selfStunTime;
    [SerializeField] private float chargeSpeed;
    private bool charging = false;
    private RaycastHit chargerHit;
    private Rigidbody rb;
    private bool attacking = false;

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

        Vector3 centre = transform.position;//change to a set height 

        Debug.DrawRay(centre, transform.TransformDirection(Vector3.forward) * 20, Color.red, 1);

        if (Physics.SphereCast(centre, 3, transform.forward, out chargerHit, 20))
        {
            if (chargerHit.collider.GetComponent<Player>())
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

    //public void Charge()
    //{
    //    charging = true;


    //    Vector3 startingPos = transform.position;
    //    Vector3 finalPos = transform.position + (transform.forward * chargeSpeed);
    //    float elapsedTime = 0;

    //    while (elapsedTime < chargeTime)
    //    {
    //        if (charging)
    //        {

    //            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / chargeTime));
    //            elapsedTime += Time.deltaTime;
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }
    //    charging = false;
    //}

    private void OnCollisionEnter(Collision collision)
    {
        charging = false;
        Debug.Log(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        charging = false;
        Debug.Log(other);
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
    }

}
