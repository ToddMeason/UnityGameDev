using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Variables
    public LayerMask enemyLayer;
    public LayerMask rockLayer;
    float speed = 10;
    float damage = 10;

    float lifeTime = 3;
    float projectileWidth = 0.1f;

    #endregion

    #region Builtin Methods
    private void Start()
    {
        Destroy(gameObject, lifeTime);

        Collider[] intialCollisions = Physics.OverlapSphere(transform.position, 0.1f, enemyLayer | rockLayer);//use | for or when checking collision masks
        if(intialCollisions.Length > 0)
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

    public void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance + projectileWidth, enemyLayer | rockLayer, QueryTriggerInteraction.Collide))
        {
            print(hit.collider.gameObject.name);
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
        GameObject.Destroy(gameObject);
    }

    #endregion
}
