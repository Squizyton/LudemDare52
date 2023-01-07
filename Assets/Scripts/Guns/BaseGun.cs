using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGun : MonoBehaviour
{
   private Bullet currentBullet;
   
   
   [Header("Base Stats")] 
   [SerializeField]private int maxAmmo;
   [SerializeField]private int currentAmmo;
   [SerializeField] private int ammoInSack;
   [SerializeField]private float reloadTime;
   [SerializeField]private float fireRate;
   
   [Header("Is Clauses")]
   [SerializeField]private bool isReloading;
   [SerializeField]private bool isAutomatic;
   [SerializeField]private bool hasInfiniteAmmo;
   
   
   
   
}
