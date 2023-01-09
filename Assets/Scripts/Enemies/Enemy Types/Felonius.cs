using System;
using System.Collections;
using System.Collections.Generic;
using Plots;
using UnityEngine;
using Random = UnityEngine.Random;

public class Felonius : BasicEnemy
{

    private PlotCell target;

    
    //eyebrow raise 
    [Header("Eating Time")]
    [SerializeField] private float eatTime;

    [SerializeField] private bool readyToGo;
    [SerializeField]private bool isEating;


    
    private void Start()
    {
        healthBar.maxValue = health;
        healthBar.value = health;
        agent.speed = speed;
        ChooseTargetPoint();
    }


    public override void OnMove(float distance)
    {
        if (!readyToGo) return;
        
        
        var distanceToTarget = Vector3.Distance(transform.position, agent.destination);
        
        if(distanceToTarget <= 0.5f)
        {
            Debug.Log("At target");
            if (target)
            {
                Debug.Log("eat time");
                state = State.Attacking;
                animator.SetFloat("speed",eatTime/100f);
            }else ChooseTargetPoint();
        }
    }


    public override void OnAttack(float distance)
    {

        if (!isEating)
        {
            isEating = true;
            animator.SetTrigger("Attack");
        }

    }
    public void AteCrop()
    {
        target.DestroyCrop();
        target.beingChargedAt = false;
        isEating = false;
        animator.speed = 1;
        ChooseTargetPoint();
        animator.SetTrigger("Run");
        target = null;
    }

    private void ChooseTargetPoint()
    {
        readyToGo = false;
        Debug.Log("Target point called");
        
        if (GameManager.Instance.grid.GetCellCountThatsGrowingSomething() > 0)
        {
            target = GameManager.Instance.grid.GetRandomCell();
            target.beingChargedAt = true;
            agent.SetDestination(target.transform.position);
            readyToGo = true;
        }
        else
        {
            if (readyToGo) return;
            
            //Choose a new Target
            var position = GameManager.Instance.currentTarget.position;
           
            //Get a random point thats a certain distance from the player
            var moveToTarget = Random.insideUnitSphere * 30;
            moveToTarget.y = transform.position.y;
            
            //Check to see if moveToTarget is in the monsterBounds
            if (GameManager.Instance.monsterBounds.bounds.Contains(new Vector3(moveToTarget.x, 0, moveToTarget.z)))
            {
               
                Debug.Log("In Bounds");
                //Move to the new target
                agent.SetDestination(moveToTarget);
                readyToGo = true;
            }
            
            
        }
        state = State.Moving;
    }
   
}
