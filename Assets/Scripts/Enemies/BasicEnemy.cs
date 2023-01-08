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

    private bool isOnFire;
    private float currentFireCooldown;

    public virtual void OnHit(float damage, bool fire = false)
    {
        if (isDead) return;
        if (fire)
        {
            isOnFire = true;
            currentFireCooldown = 10;
        }

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

    public virtual bool TicDamage()
    {
        float damage = 0f;
        if(currentFireCooldown - Time.deltaTime >= 0)
        {
            damage = Time.deltaTime;
            Debug.Log("ON FIRE:" + damage.ToString());
            currentFireCooldown -= Time.deltaTime;
        }
        else
        {
            damage = currentFireCooldown;
            currentFireCooldown = 0;
        }
        health -= damage;

        healthBar.value = health;

        if (health <= 0)
        {
            FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Enemies/Enemy_Greg/Enemy_Greg_Death", gameObject);
            OnDeath();
        }
        return currentFireCooldown > 0;
    }


    protected void Update()
    {
        if (isDead) return;

        //Handle tic damage, if any
        if (isOnFire && !TicDamage()) isOnFire = false;
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
        GameManager.Instance.RemoveEnemy();
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