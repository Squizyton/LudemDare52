using Camera;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputController : MonoBehaviour
    {
        public static PlayerInputController Instance;


       public PlayerControls playerControls;


        public CameraRotation cameraRotationClass;
    
    
        private bool isShooting;

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
            playerControls.Player.AmmoSwapping.performed += ChangeAmmo;
            playerControls.Enable();
        }


        #region Shooting
        private void Shoot()
        {
            isShooting = true;
        }


        private void Update()
        {
            if (GameManager.Instance.currentMode == GameManager.CurrentMode.TopDown) return;
        
            if (!isShooting) return;
        
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

        private void ChangeAmmo(InputAction.CallbackContext ctx)
        {
            Debug.Log("ATTEMPTING TO CHANGE AMMO");
            var gunController = PlayerInventory.Instance.currentActiveGun;
            //if (gunController.IsReloading()) return;
            gunController.SwapAmmo();
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
            BaseBullet bullet = gunController.bulletList[gunController.currentBullet];
            if (gunController.IsMagFull()) return;
            gunController.ReloadSequence(bullet.GetBulletInfo().gunReloadTime);
        }

        #endregion
        private void OnDestroy()
        {
            playerControls.Player.AmmoSwapping.performed -= ChangeAmmo;
        }
    }
}