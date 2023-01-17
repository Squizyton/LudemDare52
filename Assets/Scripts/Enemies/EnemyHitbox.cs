using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public BasicEnemy enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Bean")
            PlayerInventory.Instance.TakeDamage(enemy.transform.position, (int)enemy.GetDamage());

        other.TryGetComponent(out IHasHealth healthThing);
        if (healthThing != null)
            healthThing.TakeDamage((int)enemy.GetDamage());
    }
}