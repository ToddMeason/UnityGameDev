using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable
{
    #region Variables
    public float maxHealth;
    [Range(0, 0.9f)] public float armour;//percent based damage reduction
    protected float health;
    protected bool dead;
    protected bool hit = false;
    #endregion

    public event System.Action OnDeath;

    #region Builtin Methods
    protected virtual void Start()
    {
        health = maxHealth;
    }
    #endregion

    #region Custom Methods
    public virtual void TakeHit(float damage, RaycastHit hit)
    {
        //Do stuff with raycasthit later
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        hit = true;//Need to get this working for take hit animation on enemies
        damage = damage * armour;

        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public float healthPercent
    {
        get
        {
            return health / maxHealth;
        }
    }

    public float Heal(float healAmount)
    {
        health += healAmount;
        return health = Mathf.Clamp(health, 0f, maxHealth);//Clamp sets the min and max values for a given number/float
    }

    [ContextMenu("Self Destruct")]
    protected virtual void Die()
    {
        dead = true;
        if(OnDeath != null)
        {
            OnDeath();
        }
        Destroy(gameObject);
    }
    #endregion
}
