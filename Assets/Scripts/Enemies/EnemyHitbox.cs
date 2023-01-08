using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField]private BasicEnemy enemy;
    
  private void OnTriggerEnter(Collider other)
  {
      if (!other.gameObject.CompareTag("Player")) return;
      other.TryGetComponent(out PlayerInventory player);
      player.TakeDamage((int)enemy.GetDamage());
  }
}
