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
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Enemies/Enemy_Greg/Enemy_Greg_Death", gameObject);
            OnDeath();
        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Enemies/Enemy_Greg/Enemy_Greg_Hit", gameObject);
        }
    }


    protected void Update()
    {
        Debug.Log("Called");
        
        var distance = Vector3.Distance(transform.position, GameManager.Instance.currentTarget.position);

        switch (state)
        {
            case State.Moving:
                agent.SetDestination(GameManager.Instance.currentTarget.position);
                if (distance <= agent.stoppingDistance)
                {
                    state = State.Attacking;
                }
                break;
            case State.Attacking:
                if (distance > agent.stoppingDistance)
                {
                    animator.SetTrigger("Run");
                    state = State.Moving;
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
        //agent.enabled = false;
        healthBar.gameObject.SetActive(false);
        animator.SetTrigger("Death");
    }


    protected enum State
    {
        Moving,
        Attacking,
    }
}