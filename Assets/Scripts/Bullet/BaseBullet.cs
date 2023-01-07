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


   public void Update()
   {
      OnMove();
   }

   protected virtual void OnMove()
   {
   }

   protected virtual void OnHit()
   {
   }

   public void OnCollisionEnter(Collision collision)
   {
       OnHit();
   }
}
