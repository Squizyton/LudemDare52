using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Guns
{
    public class SpecialGun : BaseGun
    {
        [SerializeField] private PlayerInventory player;

        private void FeedStatsIntoGun(PlantInfo newAmmo)
        {
            maxAmmoPerClip = newAmmo.maxClipSize;
            fireRate = newAmmo.gunFireRate;
            isAutomatic = newAmmo.isAutomatic;
        }

        protected void SpecificGunStart()
        {
            FeedStatsIntoGun(bulletList[currentBullet].GetBulletInfo());
        }

        private void OnEnable()
        {
            ammoInSack = PlayerInventory.Instance.GetAmmo(bulletList[currentBullet].GetBulletInfo()) - GetCurrentMag();
            if (ammoInSack < 0)
                ammoInSack = 0;
            UIManager.Instance.UpdateAmmoCount(GetCurrentMag(), ammoInSack, GetIsInfinite());
            
        }


        protected override void GunStart()
        {
            throw new NotImplementedException();
        }

        public override void Shoot()
        {
            throw new NotImplementedException();
        }
    }
}