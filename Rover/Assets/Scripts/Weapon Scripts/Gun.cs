using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    #region Variables
    public Transform projectileSpawn;//Where bullet comes out ie barrel
    public Projectile projectile;
    public Rigidbody shell;
    public Transform shellEjectionSpawn;//Where shell casing is ejected
    private LineRenderer tracer;//Only used if using linerenderer instead of a bullet projectile

    public float dmg = 10;//gun damage per shot
    public float muzzleVelocity = 35;//Speed of bullet
    public float msBetweenShots = 100;//Rate of fire for gun
    public float maxMagSize = 50;//How many bullets in the guns magazine 
    public float currentMagSize;
    public float reloadSpeed = 3;//How long it takes to reload
    public bool reloading = false;
    private float nextShotTime;

    private Game_GUI gui;

    #endregion

    #region Builtin Methods
    private void Start()
    {
        gui = FindObjectOfType<Game_GUI>();

        currentMagSize = maxMagSize;
        if(GetComponent<LineRenderer>())
        {
            tracer = GetComponent<LineRenderer>();
        }
    }

    private void Update()
    {
        DisplayAmmo();
    }
    #endregion

    #region Custom Methods
    public void Shoot()
    {
        if (Time.time > nextShotTime && currentMagSize > 0 && !reloading)
        {
            //For normal bullet projectile
            //nextShotTime = Time.time + msBetweenShots / 1000;//Calculating for milliseconds
            //Projectile newProjectile = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation) as Projectile;
            //newProjectile.SetSpeed(muzzleVelocity);//Spawns bullet and sets its velcocity
            //currentMagSize--;


            //For shotgun bullet projectile
            Quaternion newRotation = projectileSpawn.rotation;
            int bulletCount = 100;
            float spread = 50;
            
            for (int i = 0; i < bulletCount; i++)
            {        
                newRotation = Quaternion.Euler(projectileSpawn.eulerAngles.x, projectileSpawn.eulerAngles.y + Random.Range(-spread, spread), projectileSpawn.eulerAngles.z);//Spawns bullets at random y angles for a shotgun spread
                Projectile newProjectile = Instantiate(projectile, projectileSpawn.position, newRotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);//Spawns bullet and sets its velcocity
            }

            nextShotTime = Time.time + msBetweenShots / 1000;//Calculating for milliseconds           
            currentMagSize--;





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

            //Play Audio
            GetComponent<AudioSource>().Play();

            //Spawning a shell casing after a shot 
            Rigidbody newShell = Instantiate(shell, shellEjectionSpawn.position, Quaternion.identity) as Rigidbody;
            newShell.AddForce(shellEjectionSpawn.forward * Random.Range(150, 200) + projectileSpawn.forward * Random.Range(-10, 10));          
        }
    }

    public void Reload()
    {
        if (!reloading && currentMagSize < maxMagSize)//Cant reload when already reloading or mag is full
        {
            reloading = true;
            StartCoroutine(ReloadGun());
        }
    }

    public void DisplayAmmo()
    {
        gui.ShowAmmo(currentMagSize);
    }

    #endregion

    #region Coroutines
    IEnumerator ReloadGun()//reload gun to maxMagSize and disable shoot while reloading
    {
        yield return new WaitForSeconds(reloadSpeed);
        currentMagSize = maxMagSize;
        reloading = false;
    }

    IEnumerator RenderTracer(Vector3 hitPoint)
    {
        tracer.enabled = true;
        tracer.SetPosition(0, projectileSpawn.position);
        tracer.SetPosition(1, projectileSpawn.position + hitPoint);
        yield return null;
        tracer.enabled = false;
    }
    #endregion
}
