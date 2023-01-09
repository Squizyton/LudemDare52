using System;
using FMOD.Studio;
using System.Collections;
using Cinemachine;
using Player;
using UI;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Guns
{
    public class BaseGun : MonoBehaviour
    {
        [Header("Important Things")] public Transform spawnPoint;

        [Header("Bullet Index")] public int currentBullet;
        [Header("AmmoTypes")] [SerializeField] public BaseBullet[] bulletList;


        [Header("Base Stats")] [SerializeField]
        protected int maxAmmoPerClip;
         [SerializeField] protected int currentMagazine;
        [SerializeField] protected int ammoInSack;
        [SerializeField] protected float reloadTime;
        [SerializeField] protected float fireRate;


        [Header("Is Clauses")] private bool isReloading;
        [SerializeField] protected bool isAutomatic;
        [SerializeField] private bool hasInfiniteAmmo;

        private bool canFire = true;
        private float totalReloadTime;
        private Vector3 hitPoint;

        //private FMOD.Studio.EventInstance GunShootSFX;
        

        public LayerMask layerMask;
        [SerializeField] private Animator animator;
        public GameObject peaParticles;

        private void Start()
        {
            UIManager.Instance.UpdateAmmoCount(currentMagazine, ammoInSack, hasInfiniteAmmo);
            UIManager.Instance.UpdateAmmoType(bulletList[currentBullet].GetBulletInfo());
            SpecificGunStart();

            //GunShootSFX = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Player/Guns/Player_Gun_Shoot");
            
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Guns/Player_Gun_Shoot"); //FMOD gun test
        }


        //Called if you want the gun to do a specific thing on start
        protected virtual void SpecificGunStart()
        {
            FeedStatsIntoGun(bulletList[currentBullet].GetBulletInfo());
            currentMagazine = 15;

            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("GunType", 0);
        }

        //Use this to change the gun stats
        //This is the base for the PEA SHOOTER. --------- Override this-------------
        protected virtual void FeedStatsIntoGun(PlantInfo info) // Pea chooter ignores plantinfo because it can't change
        {
            maxAmmoPerClip = bulletList[currentBullet].GetBulletInfo().maxClipSize;
            fireRate = bulletList[currentBullet].GetBulletInfo().gunFireRate;
        }

        public void SwapAmmo()
        {
            for (var i = 0; i < bulletList.Length - 1; i++)
            {
                var index = (i + currentBullet + 1) % bulletList.Length;
                
                var bullet = bulletList[index];
                
                if (PlayerInventory.Instance.GetAmmo(bullet.GetBulletInfo()) > 0)
                {
                    
                    currentBullet = index;
                    FMODUnity.RuntimeManager.StudioSystem.setParameterByName("GunType", currentBullet);
       
                    FeedStatsIntoGun(bullet.GetBulletInfo());
                    Debug.Log(bullet.GetBulletInfo().PlantName);
                    currentMagazine = 0;
                    ammoInSack = PlayerInventory.Instance.GetAmmo(bullet.GetBulletInfo());
                    ReloadSequence(bullet.GetBulletInfo().gunReloadTime);
                    UIManager.Instance.UpdateAmmoType(bulletList[currentBullet].GetBulletInfo());
                    UIManager.Instance.UpdateAmmoCount(0, PlayerInventory.Instance.GetAmmo(bulletList[currentBullet].GetBulletInfo()), hasInfiniteAmmo);
                    break;
                }
            }
        }


        public virtual void Shoot()
        {
            if (!canFire) return;

            if (currentMagazine > 0)
            {
                GameManager.Instance.bulletsFired++;
                animator.SetTrigger("Shoot");
                CameraShake.Shake(3, 0.1f, 0.25f);
                currentMagazine--;
                canFire = false;
                
                Debug.Log(currentMagazine);

                // Update ammo in inventory
                var currentAmmoType = bulletList[currentBullet].GetBulletInfo();
                PlayerInventory.Instance.RemoveAmmo(currentAmmoType);
                UIManager.Instance.UpdateAmmoCount(currentMagazine, ammoInSack, hasInfiniteAmmo);
                //shoot a raycast from the middle of the screen

                //rotate the bullet to face the hit point
                var position = spawnPoint.position;
                var rotation = Quaternion.LookRotation(hitPoint - position);
                //spawn the bullet
                var bullet = Instantiate(bulletList[currentBullet].gameObject, position, rotation);
                Instantiate(peaParticles, position, rotation);


                //FMOD gun type changer !!not working currentBullet!!

                //GunShootSFX.start(); 
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Guns/Player_Gun_Shoot"); //FMOD gun test

                if (IsAutomatic())
                    StartCoroutine(CoolDown());

                if (currentMagazine == 0)
                    ReloadSequence(bulletList[currentBullet].GetBulletInfo().gunReloadTime);
            }
        }


        public void ReloadSequence(float timeToReload)
        {
            
            
            Debug.Log("Reload");
            Debug.Log(ammoInSack);
            if (!hasInfiniteAmmo && ammoInSack <= 0) return;
            animator.SetTrigger("Reload");
            UIManager.Instance.ReloadGroupStatus(true, timeToReload);
            isReloading = true;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Actions/Player_Gun_Reload"); //FMOD gun test

            reloadTime = timeToReload;
            totalReloadTime = 0;
        }

        public void AbortReloadSequence()
        {
            UIManager.Instance.ReloadGroupStatus(false, 0);
            isReloading = false;
            totalReloadTime = 0;
            reloadTime = 0;
        }

        public void UpdateSack(PlantInfo info)
        {
            ammoInSack = PlayerInventory.Instance.GetAmmo(info);
         
        }


        private void Update()
        {
            //Shoot a ray from the middle of the screen
            //Camera.main is expensive, so...Idk find something to replace it
            var ray = UnityEngine.Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));


            //If we hit something
            //Store the hit point
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
            {
                hitPoint = hit.point;
            } //Developer's note: I wanna make this cleaner. It's NOT pretty

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
                    Debug.Log("reloading");
                    //get the difference between the current ammo and the max ammo
                    var difference = maxAmmoPerClip - currentMagazine;
                    Debug.Log("Difference: " + difference);

                    PlantInfo currentAmmoType = bulletList[currentBullet].GetBulletInfo();

                    //Set ammoInSack to inventory amount
                    ammoInSack = PlayerInventory.Instance.GetAmmo(currentAmmoType) - currentMagazine;
                    Debug.Log("Ammo IN Sack: " + ammoInSack);

                    //If the player has enough ammo to reload
                    if (ammoInSack >= maxAmmoPerClip)
                    {
                        ammoInSack -= difference;

                        currentMagazine = maxAmmoPerClip;

                        Debug.Log(currentMagazine);
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
                UIManager.Instance.UpdateAmmoCount(currentMagazine, ammoInSack, hasInfiniteAmmo);
            }

            #endregion
        }

        private IEnumerator CoolDown()
        {
            yield return new WaitForSeconds(fireRate);
            canFire = true;
        }

        public int GetCurrentMag()
        {
            return currentMagazine;
        }

        public bool GetIsInfinite()
        {
            return hasInfiniteAmmo;
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

        public bool IsMagFull()
        {
            return currentMagazine == maxAmmoPerClip;
        }
    }
}