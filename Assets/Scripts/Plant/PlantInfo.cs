using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Plant Info", menuName = "New Plant Info")]
public class PlantInfo : ScriptableObject
{
    public string PlantName;
    public float GrowTime;
    public GameObject plantModel;
    public GameObject plantBullet;
}
