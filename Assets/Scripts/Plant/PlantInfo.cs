using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plant Info", menuName = "New Plant Info")]
public class PlantInfo : ScriptableObject
{
    [Header("Plant Info")]
    public string PlantName;
    public float GrowTime;
    public GameObject plantModel;
    
    
    [Header("Bullet Info")]
    public GameObject plantBullet;

    public float reloadTime;
    public float bulletSpeed;
    public float bulletDamage;
    public float bulletLifeTime;
    public float bulletFireRate;
    public float bulletSpread;
    public int maxClipSize;
}
