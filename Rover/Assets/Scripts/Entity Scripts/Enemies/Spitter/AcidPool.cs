using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPool : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float damageTickRate;
    [SerializeField] private float lifeTime;
    private float damageTimer = 0;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            other.GetComponent<Player>().TakeDamage(damage);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageTickRate)
            {
                other.GetComponent<Player>().TakeDamage(damage);
                damageTimer = 0;
            }
        }
    }
}
