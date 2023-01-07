using Camera;
using UnityEngine;

namespace Player
{
    public class PlayerInputController : MonoBehaviour
    {
        public static PlayerInputController Instance;


       private PlayerControls playerControls;


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
}