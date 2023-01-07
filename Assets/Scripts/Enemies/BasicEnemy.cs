using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider)), RequireComponent(typeof(NavMeshAgent))]
public class BasicEnemy : MonoBehaviour
{
    [Header("Base Stats")] [SerializeField]
    protected float health = 5;

    [SerializeField] protected float speed;
    [SerializeField] protected float damage;

    [Header("Attack Settings")] [SerializeField]
    private float attackRate;
    private float attackTimer;

    [Header("AI")] [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected State state;

    [Header("UI")] [SerializeField] protected Slider healthBar;

    [Header("Animator")] [SerializeField] protected Animator animator;


    private bool isDead;

    public virtual void OnHit(float damage)
    {
        Debug.Log("Hit");

        if (isDead) return;

        health -= damage;

        healthBar.value = health;

        if (health <= 0)
        {
            OnDeath();
        }
    }


    protected void Update()
    {
        Debug.Log("Called");

        if (isDead) return;
        
        var distance = Vector3.Distance(transform.position, GameManager.Instance.currentTarget.position);

        switch (state)
        {
            case State.Moving:
                agent.SetDestination(GameManager.Instance.currentTarget.position);
                
               
                if (distance <= agent.stoppingDistance)
                {
                    attackTimer = attackRate;
                    state = State.Attacking;
                }
                break;
            case State.Attacking:
                if (distance > agent.stoppingDistance)
                {
                    animator.SetTrigger("Run");
                    state = State.Moving;
                }

                if (attackTimer > 0)
                {
                    attackTimer -= Time.deltaTime;
                }
                else
                {
                    animator.SetTrigger("Attack");
                    attackTimer = attackRate;
                }


                break;
            default:
                Debug.LogError("How are we out of this state machine?");
                throw new ArgumentOutOfRangeException();
        }
    }

    protected virtual void OnDeath()
    {
        isDead = true;
        agent.enabled = false;
        healthBar.gameObject.SetActive(false);
        animator.SetTrigger("Death");
        GameManager.Instance.enemiesRemaining--;
        Destroy(gameObject, 10f);
    }

    public float GetDamage()
    {
        return damage;
    }
    

    protected enum State
    {
        Moving,
        Attacking,
    }
}