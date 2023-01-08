using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public BasicEnemy enemy;

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out IHasHealth healthThing);
        if (healthThing != null)
            healthThing.TakeDamage((int) enemy.GetDamage());
    }
}