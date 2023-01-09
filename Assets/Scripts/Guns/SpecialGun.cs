using System.Collections.Generic;
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
            currentMagazine = newAmmo.maxClipSize;
            isAutomatic = newAmmo.isAutomatic;
            Debug.Log(isAutomatic);
        }
    }
}
