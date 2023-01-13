using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PepperBullet : BaseBullet
{
	[SerializeField] private GameObject hitFX;
	
   protected  override void OnHit(Transform hit)
   {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Player/Guns/Pepper/GUNS_Pepper_Explo", gameObject);   //FMOD impact

        Instantiate(hitFX, transform.position, transform.rotation);
        hit.TryGetComponent(out BasicEnemy enemy);
        
        if(enemy)
        {
         enemy.OnHit(damage, true);   
        }
        
        Destroy(gameObject);
    }
}
