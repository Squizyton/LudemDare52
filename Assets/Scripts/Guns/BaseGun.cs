using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseGun : MonoBehaviour
{
    [Header("Important Things")] public Transform spawnPoint;

    [Header("Bullet")] [SerializeField] private BaseBullet currentBullet;


    [Header("Base Stats")] [SerializeField]
    private int maxAmmo;

    [SerializeField] private int currentMagazine;
    [SerializeField] private int ammoInSack;
    [SerializeField] private float reloadTime;
    [SerializeField] private float fireRate;


    [Header("Is Clauses")] private bool isReloading;
    [SerializeField] private bool isAutomatic;
    [SerializeField] private bool hasInfiniteAmmo;
    
    private bool canFire = true;

    private void Start()
    {
        currentMagazine = 10;
    }

    public void FeedStatsIntoGun(PlantInfo info)
    {
        maxAmmo = info.maxClipSize;
        fireRate = info.gunFireRate;
        reloadTime = info.reloadTime;
    }


    public virtual void Shoot()
    {
        if (!canFire) return;

        Debug.Log("hello");

        Debug.Log(currentMagazine);
        if (currentMagazine < 0)
        {
            Debug.Log("We gucci fam?");
            currentMagazine--;
            canFire = false;

            if (IsAutomatic())
                StartCoroutine(CoolDown());
        }
        else
        {
            ReloadSequence();
        }
    }


    private void ReloadSequence()
    {
        Debug.Log("Reloading");
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


    public void SetCanFire(bool value)
    {
        this.canFire = value;
    }

    public bool CanFire()
    {
        return canFire;
    }
}