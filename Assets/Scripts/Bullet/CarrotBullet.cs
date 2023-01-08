using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotBullet : BaseBullet
{
   private int pierceCount;
   override protected void OnHit(Transform hit)
   {
        //FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Tests/gun hit", gameObject);   //FMOD impact test
        pierceCount += 1;
        hit.TryGetComponent(out BasicEnemy enemy);
        
        if(enemy)
        {
         enemy.OnHit(damage);   
        }
        if(pierceCount > 3) Destroy(gameObject);
    }
}