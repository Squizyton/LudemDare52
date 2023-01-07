using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private PlayerControls playerControls;

    private void Start()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Shoot.performed += ctx => Shoot();
    }


    private void Shoot()
    {
        
    }
}