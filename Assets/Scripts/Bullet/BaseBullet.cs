using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
   [Header("Bullet Info")]
   [SerializeField] private PlantInfo bulletInfo;
   
   
   
   [Header("Bullet Stats")]
   [SerializeField] private float speed;
   [SerializeField] private float damage;

   [Header("Other")] [SerializeField] private Rigidbody rb;
   [SerializeField]private LayerMask whatIsEnemy;

   [SerializeField] private float bulletRaycastLength;
   private void Start()
   {
       speed = bulletInfo.bulletSpeed;
       damage = bulletInfo.bulletDamage;
       
       rb.velocity = transform.forward * speed;
   }


   public void Update()
   {
       if(Physics.Raycast(transform.position, transform.forward, out var hit,bulletRaycastLength,whatIsEnemy))
       {
           OnHit(hit.transform);
       }
   }

   public void FixedUpdate()
   {
      OnMove();
   }

   protected virtual void OnMove()
   {
      //move the bullet foward
      
   }

   protected virtual void OnHit(Transform hit)
   {
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/Tests/gun hit", gameObject);   //FMOD impact test
        
        hit.TryGetComponent(out BasicEnemy enemy);
        
        if(enemy)
        {
         enemy.OnHit(damage);   
        }
        
        Destroy(gameObject);
   }

   private void OnDrawGizmos()
   {
         Gizmos.color = Color.red;
         Gizmos.DrawRay(transform.position, transform.forward * bulletRaycastLength);
   }

   public PlantInfo GetBulletInfo()
   {
       return bulletInfo;
   }
}
