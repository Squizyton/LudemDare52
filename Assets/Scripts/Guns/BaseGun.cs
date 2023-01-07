using System.Collections;
using UI;
using UnityEngine;

namespace Guns
{
    public class BaseGun : MonoBehaviour
    {
        [Header("Important Things")] public Transform spawnPoint;

        [Header("Bullet")] public BaseBullet currentBullet;


        [Header("Base Stats")] [SerializeField]
        private int maxAmmoPerClip;

        [SerializeField] private int currentMagazine;
        [SerializeField] private int ammoInSack;
        [SerializeField] private float reloadTime;
        [SerializeField] private float fireRate;


        [Header("Is Clauses")] private bool isReloading;
        [SerializeField] private bool isAutomatic;
        [SerializeField] private bool hasInfiniteAmmo;

        private bool canFire = true;
        private float totalReloadTime;
        private Vector3 hitPoint;
        private void Start()
        {
            currentMagazine = 10;
            SpecificGunStart();
        }

        protected virtual void SpecificGunStart()
        {
            FeedStatsIntoGun(currentBullet.GetBulletInfo());
        }

        private void FeedStatsIntoGun(PlantInfo info)
        {
            maxAmmoPerClip = info.maxClipSize;
            fireRate = info.gunFireRate;
        }


        public virtual void Shoot()
        {
            if (!canFire) return;

            if (currentMagazine > 0)
            {
                currentMagazine--;
                canFire = false;

                //shoot a raycast from the middle of the screen


                //rotate the bullet to face the hit point
                var position = spawnPoint.position;
                Instantiate(currentBullet, position,
                    Quaternion.LookRotation(hitPoint - position));

                if (IsAutomatic())
                    StartCoroutine(CoolDown());
            }
            else
            {
                ReloadSequence(currentBullet.GetBulletInfo().gunReloadTime);
            }
        }


        public void ReloadSequence(float timeToReload)
        {
            UIManager.Instance.ReloadGroupStatus(true, timeToReload);
            isReloading = true;
            reloadTime = timeToReload;
            totalReloadTime = 0;
        }


        private void Update()
        {
         
            var ray = PlayerInputController.Instance.cameraRotationClass.GetMainCamera().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            
            if (!Physics.Raycast(ray.origin, transform.forward, out var hit, 10000)) return;
            {
                hitPoint = hit.point;
            }

            //Developer's note: I wanna make this cleaner. It's NOT pretty
            #region Reloading

            if (!isReloading) return;
            if (reloadTime > 0f)
            {
                reloadTime -= Time.deltaTime;
                totalReloadTime += Time.deltaTime;
                UIManager.Instance.FeedReloadTime(totalReloadTime);
            }
            else
            {
                if (!hasInfiniteAmmo)
                {
                    //get the difference between the current ammo and the max ammo
                    var difference = maxAmmoPerClip - currentMagazine;


                    //If the player has enough ammo to reload
                    if (ammoInSack >= maxAmmoPerClip)
                    {
                        ammoInSack -= difference;
                        currentMagazine = maxAmmoPerClip;
                    }
                    else
                    {
                        currentMagazine = ammoInSack;
                        ammoInSack = 0;
                    }
                }
                else
                {
                    currentMagazine = maxAmmoPerClip;
                }

                isReloading = false;
                UIManager.Instance.ReloadGroupStatus(false, 0);
            }

            #endregion
        }

        private IEnumerator CoolDown()
        {
            yield return new WaitForSeconds(fireRate);
            canFire = true;
        }


        public bool IsAutomatic()
        {
            return isAutomatic;
        }

        public bool IsReloading()
        {
            return isReloading;
        }


        public void SetCanFire(bool value)
        {
            this.canFire = value;
        }


        public bool CanFire()
        {
            return canFire;
        }


      
    }
}