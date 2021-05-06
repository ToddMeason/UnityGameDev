using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour, IDamageable
{
    #region Variables
    public float startingHealth;
    protected float health;
    protected bool dead;
    #endregion

    public event System.Action OnDeath;

    #region Builtin Methods
    protected virtual void Start()
    {
        health = startingHealth;
    }

    #endregion

    #region Custom Methods
    public virtual void TakeHit(float damage, RaycastHit hit)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

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
