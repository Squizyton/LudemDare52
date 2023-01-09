using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PepperBullet : BaseBullet
{

   override protected void OnHit(Transform hit)
   {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Player/Guns/Pepper_Flame", gameObject);   //FMOD impact test
        
        hit.TryGetComponent(out BasicEnemy enemy);
        
        if(enemy)
        {
         enemy.OnHit(damage, true);   
        }
        
        Destroy(gameObject);
    }
}
