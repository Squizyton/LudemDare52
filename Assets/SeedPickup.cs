using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedPickup : MonoBehaviour
{
   public PlantInfo plantInfo;
   public Image image;

   private void OnTriggerEnter(Collider other)
   {
      if (!other.CompareTag("Player")) return;
      
      other.TryGetComponent(out PlayerInventory inventory);
      inventory.AddSeed(PlayerInventory.Instance.seedInventory,plantInfo,plantInfo.seedYield);
      Destroy(gameObject);
   }
}
