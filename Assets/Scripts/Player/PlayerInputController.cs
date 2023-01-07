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
        playerControls.Player.Reload.performed += ctx => OnReload();
        playerControls.Enable();
    }


    #region Shooting
    private void Shoot()
    {
        isShooting = true;
    }


    private void Update()
    {
        if (!isShooting) return;
        
    
        //TODO: These two are redundant. Fix it.
        if (!PlayerInventory.Instance.currentActiveGun.IsReloading()) return;    
        var gunController = PlayerInventory.Instance.currentActiveGun;

        if (gunController.IsAutomatic())
        {
            gunController.Shoot();
        }else if (gunController.CanFire())
        {
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
    
    private void OnReload()
    {
        
        
        var gunController = PlayerInventory.Instance.currentActiveGun;
        
        if(gunController.IsReloading()) return;
        gunController.ReloadSequence(gunController.currentBullet.GetBulletInfo().gunReloadTime);
    }
    
    #endregion
}