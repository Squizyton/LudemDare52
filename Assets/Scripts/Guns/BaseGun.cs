using System;
using FMOD.Studio;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Camera;
using Cinemachine;
using Player;
using UI;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Guns
{
    public abstract class BaseGun : MonoBehaviour
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
        [SerializeField] protected  bool hasInfiniteAmmo;

        private int SpecialGunBullet;
        protected bool canFire = true;
        private float totalReloadTime;
        protected Vector3 hitPoint;


        public LayerMask layerMask;
        [SerializeField] protected Animator animator;
        public GameObject peaParticles;

        
        //Async stuff
        private Task _task;
        private CancellationTokenSource _tokenSource;
        
        
        private static readonly int Reload = Animator.StringToHash("Reload");

        #region Start Functions
        
        protected abstract void GunStart();

        #endregion

        #region Shooting Functions

        public abstract void Shoot();

        protected void GetRaycastHit()
        {
            //Shoot a ray from the middle of the screen
            //Camera.main is expensive, so...Idk find something to replace it
            var ray = PlayerInputController.Instance.cameraRotationClass.GetMainCamera().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            //If we hit something
            //Store the hit point
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask))
            {
                hitPoint = hit.point;
            }
        }

        #endregion

        #region Reload

        public async void StartReload(float time)
        {
            if (isReloading) return;
            _tokenSource = new CancellationTokenSource();
            _task = ReloadSequence(time, _tokenSource.Token);
            Debug.Log("Reloading");
            await _task;
            Debug.Log("Reload Complete");
        }

        protected virtual async Task ReloadSequence(float timeToReload, CancellationToken token)
        {
            if (!hasInfiniteAmmo && ammoInSack <= 0) return;


            var completed = false;
            totalReloadTime = 0;
            var time = timeToReload;
            UIManager.Instance.ReloadGroupStatus(true, time);
            
            isReloading = true;
            animator.SetTrigger(Reload);

            while (!completed)
            {

                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (time > 0)
                {
                    time -= Time.deltaTime;
                    totalReloadTime += Time.deltaTime;
                    UIManager.Instance.FeedReloadTime(totalReloadTime);
                }
                else completed = true;

                await Task.Yield();
            }


            if (!hasInfiniteAmmo)
            {
                //get the difference between the current ammo and the max ammo
                var difference = maxAmmoPerClip - currentMagazine;

                var currentAmmoType = bulletList[currentBullet].GetBulletInfo();

                //Set ammoInSack to inventory amount
                ammoInSack = PlayerInventory.Instance.GetAmmo(currentAmmoType) - currentMagazine;

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
            UIManager.Instance.ReloadGroupStatus(false);
            UIManager.Instance.UpdateAmmoCount(currentMagazine, ammoInSack, hasInfiniteAmmo);
        }

        public void AbortReloadSequence()
        {
            //Abort the reload sequence
            isReloading = false;
            UIManager.Instance.ReloadGroupStatus(false, 0);
            totalReloadTime = 0;
            //Kill the task to free up memory
            _tokenSource?.Cancel();
        }


        #endregion


        #region Setters/Getters

        
        public void UpdateSack(PlantInfo info)
        {
            ammoInSack = PlayerInventory.Instance.GetAmmo(info);
        }
        
        protected IEnumerator CoolDown()
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

        #endregion
    }
}