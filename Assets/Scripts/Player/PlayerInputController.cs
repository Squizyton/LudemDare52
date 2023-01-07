using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private PlayerControls playerControls;

    
    
    bool isShooting = false;
    private void Start()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Shoot.performed += ctx => Shoot();
        playerControls.Player.Shoot.canceled += ctx => StopShoot();
        playerControls.Enable();
    }


    #region Shooting
    private void Shoot()
    {
        isShooting = true;
    }


    private void Update()
    {

        var gunController = PlayerInventory.Instance.currentActiveGun;
        
        
        if (gunController.IsAutomatic() && isShooting)
        {
            Debug.Log("Gun is automatic and is shooting");
            gunController.Shoot();
        }else if (gunController.CanFire() && isShooting)
        {
            Debug.Log("Semi Automatic");
            gunController.Shoot();
            gunController.SetCanFire(false);
        }

    }

    private void StopShoot()
    {
        var gunController = PlayerInventory.Instance.currentActiveGun;
        
        if(!gunController.IsAutomatic())
            gunController.SetCanFire(true);


        isShooting = false;
    }
    #endregion
}