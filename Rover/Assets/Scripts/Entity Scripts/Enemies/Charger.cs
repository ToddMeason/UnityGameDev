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

    public override void CheckAttack()
    {
        //Needs to constantly face player except when charging
        //spherecast forward and see if it can hit the player in a straight line

        Vector3 centre = transform.position;//change to a set height 
        
        Debug.DrawRay(centre, transform.TransformDirection(Vector3.forward) * 20, Color.red, 1);

        if (Physics.SphereCast(centre, 1, transform.forward, out chargerHit, 20))
        {
            if (chargerHit.collider.GetComponent<Player>())
            {
                Debug.Log("Detect Player");
                StartCoroutine(Attack());
            }
            else
            {
                Debug.Log("Detect Object Not player");
            }          
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        charging = false;
        Debug.Log(collision);
    }

    public override IEnumerator Attack()
    {
        pathfinder.enabled = false;
        //play roar/starting charge animation
        yield return new WaitForSeconds(windUpTime);

        charging = true;
        //play charge animation

        Vector3 startingPos = transform.position;
        Vector3 finalPos = transform.position + (transform.forward * chargeSpeed);
        float elapsedTime = 0;

        while (elapsedTime < chargeTime)
        {
            if (charging)
            {
                transform.position += transform.forward * Time.deltaTime * chargeSpeed;//not too bad
                //transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / chargeTime));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            else
            {
                break;
            }         
        }
        charging = false;

        //transform.position = Vector3.MoveTowards(transform.position, chargerHit.transform.position, 2);
        //charge forward for x amount of time

        //charge forward in a straight line at the most recent player position dealing damage along the way
        yield return new WaitForSeconds(selfStunTime);
        pathfinder.enabled = true;
    }

}
