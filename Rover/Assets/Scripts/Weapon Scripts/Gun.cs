using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    #region Variables
    public Transform bulletSpawn;//Where bullet comes out ie barrel
    public Projectile projectile;
    public Rigidbody shell;
    public Transform shellEjectionSpawn;//Where shell casing is ejected
    private LineRenderer tracer;//Only used if using linerenderer instead of a bullet projectile

    public float dmg = 10;//gun damage per shot
    public float muzzleVelocity = 35;//Speed of bullet
    public float msBetweenShots = 100;//Rate of fire for gun
    private float nextShotTime;

    #endregion

    #region Builtin Methods
    private void Start()
    {
        if(GetComponent<LineRenderer>())
        {
            tracer = GetComponent<LineRenderer>();
        }
    }
    #endregion

    #region Custom Methods
    public void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            //For bullet projectile
            nextShotTime = Time.time + msBetweenShots / 1000;//Calculating for milliseconds
            Projectile newProjectile = Instantiate(projectile, bulletSpawn.position, bulletSpawn.rotation) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);//Spawns bullet and sets its velcocity

            ////For LineRenderer 'projectile'
            //Ray ray = new Ray(transform.position, transform.forward);
            //RaycastHit hit;

            //float shotDistance = 20;

            //if(Physics.Raycast(ray, out hit, shotDistance))
            //{
            //    shotDistance = hit.distance;
            //}

            //if (tracer)
            //{
            //    StartCoroutine(RenderTracer(ray.direction * shotDistance));
            //}
            ////Above for lineRenderer
            
            GetComponent<AudioSource>().Play();

            Rigidbody newShell = Instantiate(shell, shellEjectionSpawn.position, Quaternion.identity) as Rigidbody;//Spawning a shell casing after a shot 
            newShell.AddForce(shellEjectionSpawn.forward * Random.Range(150, 200) + bulletSpawn.forward * Random.Range(-10, 10));
        }
    }

    #endregion

    #region Coroutines

    IEnumerator RenderTracer(Vector3 hitPoint)
    {
        tracer.enabled = true;
        tracer.SetPosition(0, bulletSpawn.position);
        tracer.SetPosition(1, bulletSpawn.position + hitPoint);
        yield return null;
        tracer.enabled = false;
    }
    #endregion
}
