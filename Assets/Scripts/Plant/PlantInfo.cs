using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Plant Info", menuName = "New Plant Info")]
public class PlantInfo : ScriptableObject
{
    [Header("Plant Info")]
    public string PlantName;
    public float GrowTime;
    public int seedYield;
    public int plantCost;
    public GameObject plantModel;
    public Sprite seedIcon;
    public Sprite bulletIcon;
    [Header("Bullet Info")]
    public float bulletSpeed;
    public float bulletDamage;
    public float bulletLifeTime;
    public string FmodBulletName;    //to be discontinued
    [Header("Gun Info")] 
    public bool isAutomatic;
    public float gunFireRate;
    public float gunBulletSpread;
    public int maxClipSize;
    public float gunReloadTime;
    [Header("FMOD Info")]
    public FMODUnity.EventReference fmodGunFire;
    public FMODUnity.EventReference fmodAltFire;
    public FMODUnity.EventReference fmodReload;
}
