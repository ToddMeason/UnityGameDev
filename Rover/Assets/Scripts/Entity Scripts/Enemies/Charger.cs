using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Enemy
{
    public override void CheckAttack()
    {
        //spherecast forward and see if it can hit the player in a straight line 
    }

    public override IEnumerator Attack()
    {
        throw new System.NotImplementedException();
        //wind up a charge animation 
        //charge forward in a straight line at the most recent player position dealing damage along the way
    }

}
