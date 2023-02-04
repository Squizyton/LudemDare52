using UI;
using UnityEngine;

namespace Guns
{
    public class PeaShooter : BaseGun
    {
        // Start is called before the first frame update

        public GameObject debugSphere;
    
    
        private new void Start()
        {
            GunStart();
        }

        protected override void GunStart()
        {
            FeedStatsIntoGun(bulletList[currentBullet].GetBulletInfo());
            currentMagazine = 15;
        }


        void FeedStatsIntoGun(PlantInfo newAmmo)
        {
            maxAmmoPerClip = newAmmo.maxClipSize;
            fireRate = newAmmo.gunFireRate;
            isAutomatic = newAmmo.isAutomatic;
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
            //FmodShootSound();

            if (IsAutomatic())
                StartCoroutine(CoolDown());

            if (currentMagazine == 0)
               await ReloadSequence(bulletList[currentBullet].GetBulletInfo().gunReloadTime);
            //else
            //FmodNoAmmo();
        }

        // Update is called once per frame
        void Update()
        {
            GetRaycastHit();
            debugSphere.transform.position = hitPoint;
        }
    }
}