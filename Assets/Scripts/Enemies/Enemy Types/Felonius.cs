using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
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
    [SerializeField] private bool isEating;

    [SerializeField] private bool ifDoNotPlaySound;

    [SerializeField] private FMODUnity.EventReference FmodFootstepEvent;
    [SerializeField] private FMODUnity.EventReference FmodBodyfallEvent;
    [SerializeField] private FMODUnity.EventReference FmodDeathEvent;
    [SerializeField] private GameObject head;



    private void Start()
    {
        healthBar.maxValue = health;
        healthBar.value = health;
        agent.speed = speed;
        ChooseTargetPoint();
        if (head == null) head = gameObject;
    }


    public override void OnMove(float distance)
    {
        if (!readyToGo) return;
        
        
        var distanceToTarget = Vector3.Distance(transform.position, agent.destination);
        
        if(distanceToTarget <= 0.5f)
        {
            if (target)
            {
                state = State.Attacking;;
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

    #region FMOD
    public void FmodPostFootstepsEvent()
    {
        PlaySound(FmodFootstepEvent, gameObject);
    }

    public void DevourSound()
    {
        PlaySound("event:/SFX/Enemy/Felonious/Enemy_Felonious_Devour");
    }
    public void BodyfallSound()
    {
        PlaySound(FmodBodyfallEvent, gameObject);
    }
    public void FmodPostDeathEvent()
    {
        PlaySound(FmodDeathEvent, head);
    }

    public void PlaySound(string sound)
    {
        if (!ifDoNotPlaySound) FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);   //FMOD
    }

    public void PlaySound(FMODUnity.EventReference sound, GameObject source)
    {
        if (!ifDoNotPlaySound) FMODUnity.RuntimeManager.PlayOneShotAttached(sound, source);   //FMOD
    }
    #endregion
}
