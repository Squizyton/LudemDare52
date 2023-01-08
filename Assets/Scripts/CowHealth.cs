using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowHealth : MonoBehaviour, IHasHealth
{
    public int health;
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
           //TODO: Game Over!
        }
    }
}
