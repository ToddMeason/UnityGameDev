using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    //Exists so collision can just check for this instead of check every script that has a TakeHit method
    void TakeHit(float damage, RaycastHit hit);
}
