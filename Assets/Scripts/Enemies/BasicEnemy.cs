using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody)),RequireComponent(typeof(CapsuleCollider)),RequireComponent(typeof(NavMeshAgent))]
public class BasicEnemy : MonoBehaviour
{

    [Header("Base Stats")] 
    [SerializeField] protected float health = 5;

    [SerializeField] private float speed;
    [SerializeField] private float damage;

    [Header("Attack Settings")] [SerializeField]
    private float attackRate;
    
    
    [Header("AI")]
    [SerializeField]private NavMeshAgent agent;

    [Header("UI")] [SerializeField] protected Slider healthBar;
    
    [Header("Animator")][SerializeField] private Animator animator;


    private bool isDead;

    public virtual void OnHit(float damage)
    {
        Debug.Log("Hit");
        
        if (isDead) return;
        
        health -= damage;
        
       healthBar.value = health;
        
        if(health <= 0)
        {
           OnDeath();
        }
    }


    protected virtual void OnDeath()
    {
        isDead = true;
        //agent.enabled = false;
        healthBar.gameObject.SetActive(false);
        animator.SetTrigger("Death");
    }
}
