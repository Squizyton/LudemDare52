using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowHealth : MonoBehaviour, IHasHealth
{
    public int health;
    
    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        if (health <= 0)
        {
           //TODO: Game Over!
        }
    }
}
