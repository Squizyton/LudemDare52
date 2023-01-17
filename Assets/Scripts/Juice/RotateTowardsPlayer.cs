using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RotateTowardsPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;


    private void Start()
    {
        player = GameManager.Instance.currentTarget;
    }

    private void Update()
    {
 
     transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(player.position - transform.position), Time.deltaTime * 2f);   
    }
}
