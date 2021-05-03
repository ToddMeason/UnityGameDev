using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region Variables
    public float health;
    #endregion

    #region Builtin Methods
    public virtual void Start()
    {

    }

    #endregion

    #region Custom Methods
    public virtual void TakeDamage(float dmg)
    {
        health -= dmg;


        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {

        Destroy(gameObject);
    }
    #endregion
}
