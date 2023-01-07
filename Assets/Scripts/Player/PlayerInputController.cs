using System;
using System.Collections;
using System.Collections.Generic;
using Camera;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public static PlayerInputController Instance;


    [SerializeField] private PlayerControls playerControls;


    public CameraRotation cameraRotationClass;
    
    
    bool isShooting = false;

    private void Awake()
    {
        Instance = this;
    }

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
        
    
        Debug.Log("Uh");
        
        //TODO: These two are redundant. Fix it.
        if (PlayerInventory.Instance.currentActiveGun.IsReloading()) return;    
        var gunController = PlayerInventory.Instance.currentActiveGun;

        if (gunController.IsAutomatic())
        {
            gunController.Shoot();
        }
        else if (gunController.CanFire())
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