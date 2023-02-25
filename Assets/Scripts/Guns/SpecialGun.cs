using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Guns
{
    public class SpecialGun : BaseGun
    {
        [SerializeField] private PlayerInventory player;

        protected override void GunStart()
        {
        }

        private void OnEnable()
        {
            ammoInSack = PlayerInventory.Instance.GetAmmo(bulletList[currentBullet].GetBulletInfo()) - GetCurrentMag();
            if (ammoInSack < 0)
                ammoInSack = 0;
            UIManager.Instance.UpdateAmmoCount(GetCurrentMag(), ammoInSack, GetIsInfinite());
        }

        private void FeedStatsIntoGun(PlantInfo newAmmo)
        {
            maxAmmoPerClip = newAmmo.maxClipSize;
            fireRate = newAmmo.gunFireRate;
            isAutomatic = newAmmo.isAutomatic;
            fmodBullet = newAmmo.FmodBulletName;    //to be discontinued
            fmodGunFire = newAmmo.fmodGunFire;
        }

        protected void SpecificGunStart()
        {
            FeedStatsIntoGun(bulletList[currentBullet].GetBulletInfo());
        }

        private void Update()
        {
           GetRaycastHit();
        }

        public override async void Shoot()
        {
            if (!canFire) return;

            if (currentMagazine <= 0) return;

            GameManager.Instance.bulletsFired++;
            animator.SetTrigger("Shoot");
            CameraShake.Shake(3, 0.1f, 0.25f);
            currentMagazine--;
            canFire = false;

            // Update ammo in inventory
            var currentAmmoType = bulletList[currentBullet].GetBulletInfo();
            PlayerInventory.Instance.RemoveAmmo(currentAmmoType);
            UIManager.Instance.UpdateAmmoCount(currentMagazine, ammoInSack, hasInfiniteAmmo);

            //rotate the bullet to face the hit point
            var position = spawnPoint.position;
            var rotation = Quaternion.LookRotation(hitPoint - position);

            //spawn the bullet
            var bullet = Instantiate(bulletList[currentBullet].gameObject, position, rotation);
            Instantiate(peaParticles, position, rotation);

            //FMOD
            FmodShootSound();

            if (IsAutomatic())
                StartCoroutine(CoolDown());

            if (currentMagazine == 0)
            {
                StartReload(bulletList[currentBullet].GetBulletInfo().gunReloadTime);
                FmodNoAmmo();
            }
        }



        public void SwapAmmo()
        {
            //Get the new index
            var index = (currentBullet + 1) % bulletList.Length;
            
            //Get the new ammo type
            var bullet = bulletList[index].GetBulletInfo();

            //Set current bullet to the new index
            currentBullet = index;
            
            //If we are already reloading, abort the reload
            AbortReloadSequence();
            
            //Set the current magazine to 0
            currentMagazine = 0;
            FeedStatsIntoGun(bullet);
            
            //Start the reload sequence
            ammoInSack = PlayerInventory.Instance.GetAmmo(bullet);
            StartReload(bullet.gunReloadTime);
            
            //Update the UI
            UIManager.Instance.UpdateAmmoType(bullet);
            UIManager.Instance.UpdateAmmoCount(0, PlayerInventory.Instance.GetAmmo(bulletList[currentBullet].GetBulletInfo()), hasInfiniteAmmo);

            //Change FMOD ammo type
            ChangeFMODGunType();    //to be discontinued
        }

    }
}