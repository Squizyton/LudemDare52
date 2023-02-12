using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Turret Info", menuName = "New Turret Info")]
public class TurretInfo : ScriptableObject
{
    [Header("Turret Info")]
    public float DecayTime;
    public GameObject plantModel;
    [Header("Bullet Info")]
    public GameObject bulletPrefab;
    public string FmodBulletName;
    [Header("Gun Info")]
    public float gunFireRate;
    public float gunBulletSpread;
    public float Range;
}
