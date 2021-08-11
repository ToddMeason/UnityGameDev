using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour
{
    #region Variables
    public enum WeaponType {MachineGun, ShotGun};//not used currently will be needed for rocket launcher later maybe
    public WeaponType weaponType;

    public Transform projectileSpawn;//Where bullet comes out ie barrel
    public Projectile projectile;
    public Rigidbody shell;
    public Transform shellEjectionSpawn;//Where shell casing is ejected
    private LineRenderer tracer;//Only used if using linerenderer instead of a bullet projectile

    //May need to change type or make a list/enum later, very scuffed way of doing/passing stats
    public float dmg = 10;            public float dmgBonus = 0;             public float dmgTotal;                 //gun damage per shot
    public float muzzleVelocity = 35; public float muzzleVelocityBonus = 0;  public float muzzleVelocityTotal;      //Speed of bullet
    public float msBetweenShots = 100;public float msBetweenShotsBonus = 0;  public float msBetweenShotsTotal;     //Rate of fire for gun, smaller is faster
    public float maxMagSize = 50;     public float maxMagSizeBonus = 0;      public float maxMagSizeTotal;          //How many bullets in the guns magazine 
    public float currentMagSize;                                                 
    public float reloadSpeed = 3;     public float reloadSpeedBonus = 0;     public float reloadSpeedTotal;          //How long it takes to reload, smaller is faster
    public float projectileCount = 1; public float projectileCountBonus = 0; public float projectileCountTotal;      //Amount of projectiles fired per shot
    public float spread = 5;          public float spreadBonus = 0;          public float spreadTotal;               //Accuracy or spread of bullets, smaller is more accurate
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
        SetTotals();
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
            Quaternion newRotation = projectileSpawn.rotation;
                    
            for (int i = 0; i < projectileCountTotal; i++)
            {
                newRotation = Quaternion.Euler(projectileSpawn.eulerAngles.x, projectileSpawn.eulerAngles.y + Random.Range(-spreadTotal, spreadTotal), projectileSpawn.eulerAngles.z);//Spawns bullets at random y angles for a random spread
                Projectile newProjectile = Instantiate(projectile, projectileSpawn.position, newRotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocityTotal);//Spawns bullet and sets its velcocity
                newProjectile.SetDamage(dmgTotal);
            }

            nextShotTime = Time.time + msBetweenShotsTotal / 1000;//Calculating for milliseconds           
            currentMagSize--;
            //Above for normal bullet projectile

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
        if (!reloading && currentMagSize < maxMagSizeTotal)//Cant reload when already reloading or mag is full
        {
            reloading = true;
            StartCoroutine(ReloadGun());
        }
    }

    public void DisplayAmmo()
    {
        gui.ShowAmmo(currentMagSize);
    }

    public void SetTotals()
    {
        dmgTotal = dmg + dmgBonus;
        muzzleVelocityTotal = muzzleVelocity + muzzleVelocityBonus;
        msBetweenShotsTotal = msBetweenShots + msBetweenShotsBonus;
        maxMagSizeTotal = maxMagSize + maxMagSizeBonus;
        reloadSpeedTotal = reloadSpeed + reloadSpeedBonus;
        projectileCountTotal = projectileCount + projectileCountBonus;
        spreadTotal = spread + spreadBonus;
    }

    #endregion

    #region Events
    private void OnEnable()
    {
        InventoryObject.pickedUpItem += SetTotals;
        Rover.Basic.Rover_GunController.GunEquipped += SetTotals;
    }

    private void OnDisable()
    {
        InventoryObject.pickedUpItem -= SetTotals;
        Rover.Basic.Rover_GunController.GunEquipped -= SetTotals;
    }
    #endregion

    #region Coroutines
    IEnumerator ReloadGun()//reload gun to maxMagSize and disable shoot while reloading
    {
        yield return new WaitForSeconds(reloadSpeedTotal);
        currentMagSize = maxMagSizeTotal;
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