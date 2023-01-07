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
   
   
   private void Start()
   {
       speed = bulletInfo.bulletSpeed;
       damage = bulletInfo.bulletDamage;
   }

   
   
   

   public void FixedUpdate()
   {
      OnMove();
   }

   protected virtual void OnMove()
   {
       transform.Translate(transform.forward * (speed * Time.deltaTime));
   }

   protected virtual void OnHit()
   {
       Destroy(gameObject);
   }

   public void OnCollisionEnter(Collision collision)
   {
       OnHit();
   }
}
