using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Guns
{
    public class SpecialGun : BaseGun
    {
        [SerializeField] private PlayerInventory player;

        protected override void FeedStatsIntoGun(PlantInfo newAmmo)
        {
            maxAmmoPerClip = newAmmo.maxClipSize;
            fireRate = newAmmo.gunFireRate;
            isAutomatic = newAmmo.isAutomatic;
            Debug.Log(isAutomatic);
        }

        protected override void SpecificGunStart()
        {
            FeedStatsIntoGun(bulletList[currentBullet].GetBulletInfo());
        }

        private void OnEnable()
        {
            ammoInSack = PlayerInventory.Instance.GetAmmo(bulletList[currentBullet].GetBulletInfo());
            UIManager.Instance.UpdateAmmoCount(GetCurrentMag(), ammoInSack, GetIsInfinite());
            
        }
    }
}