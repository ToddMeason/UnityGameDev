using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitterProjectile : MonoBehaviour
{
    #region Variables
    public LayerMask playerLayer;
    public LayerMask rockLayer;
    public LayerMask groundLayer;

    [SerializeField] private GameObject acidPool;

    private float speed = 10;
    private float damage = 1;
    [SerializeField] private float lifeTime = 3;
    [SerializeField] private float projectileWidth = 0.3f;

    #endregion

    #region Builtin Methods
    private void Start()
    {
        Destroy(gameObject, lifeTime);

        Collider[] intialCollisions = Physics.OverlapSphere(transform.position, 0.1f, playerLayer | rockLayer | groundLayer);//use | for or when checking collision masks
        if (intialCollisions.Length > 0)
        {
            OnHitObject(intialCollisions[0]);
        }
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    #endregion

    #region Custom Methods
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + projectileWidth, playerLayer | rockLayer | groundLayer, QueryTriggerInteraction.Collide))
        {
            //print(hit.collider.gameObject.name);
            OnHitObject(hit);
        }
    }

    public void OnHitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hit);
        }
        //print(hit.collider.gameObject.name);
        Instantiate(acidPool, transform.position, Quaternion.identity);//calculate position to ground then spawn acidpool on the ground

        GameObject.Destroy(gameObject);
    }

    public void OnHitObject(Collider col)
    {
        IDamageable damageableObject = col.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }
        //print(col.gameObject.name);
        Instantiate(acidPool, transform.position, Quaternion.identity);//calculate position to ground then spawn acidpool on the ground
        GameObject.Destroy(gameObject);
    }

    #endregion
}
