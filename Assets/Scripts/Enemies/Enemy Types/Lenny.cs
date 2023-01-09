using FMOD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lenny : BasicEnemy
{
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform[] spawnPoints;

    private bool atTarget;
    private Vector3 moveToTarget;

    private bool attacked;

    
    public void Start()
    {
        agent.speed = speed;
        healthBar.maxValue = health;
        healthBar.value = health;
        atTarget = true;
    }

    public override void OnMove(float distance)
    {
        //find a spot to move to thats a certain distance from the player
        if (atTarget)
        {
            //Choose a new Target
            var position = GameManager.Instance.currentTarget.position;
            //Get a random point thats a certain distance from the player
            moveToTarget = position + Random.insideUnitSphere * 40;
            moveToTarget.y = transform.position.y;
            


            //Check to see if moveToTarget is in the monsterBounds
            if (GameManager.Instance.monsterBounds.bounds.Contains(new Vector3(moveToTarget.x, 0, moveToTarget.z)))
            {
                UnityEngine.Debug.Log("In Bounds");
                //Move to the new target
                atTarget = false;
            }
        }

        if (atTarget) return;
        //move to that spot
        if (agent.enabled)
            agent.SetDestination(new Vector3(moveToTarget.x, transform.position.y, moveToTarget.z));

        var distanceToTarget = Vector3.Distance(transform.position, moveToTarget);

        if (!(distanceToTarget < 0.5f)) return;

        UnityEngine.Debug.Log("At Target");

        //Change the mode
        state = State.Attacking;
    }

    public override void OnAttack(float distance)
    {
        var distanceFromPlayer = Vector3.Distance(transform.position, GameManager.Instance.currentTarget.position);
        //
        if (distance < 10)
        {
            animator.SetTrigger("Run");
            atTarget = true;

            state = State.Moving;
        }

        if (attackTimer > 0 && attacked)
        {
            //rotate to face the player + the forward vector of the player * 2
            var targetRotation =
                Quaternion.LookRotation(GameManager.Instance.currentTarget.position - transform.position);
            //rotate to face the target
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
            attackTimer -= Time.deltaTime;
        }
        else
        {
            animator.SetTrigger("Attack");
            SetAttack();
        }
    }


    public void SpawnProjectile(int spawnindex)
    {
        Instantiate(_bullet, spawnPoints[spawnindex].position, Quaternion.identity)
            .TryGetComponent(out EnemyHitbox bhitbox);
        bhitbox.enemy = this;

        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Enemies/Enemy_Lenny/Enemy_Lenny_Attack", gameObject);   //FMOD

    }

    public void SetAttack()
    {
        attackTimer = attackRate;
        attacked = true;
    }
}