using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster Spawn", menuName = "Monster Spawn")]
public class MonsterObject : ScriptableObject
{

    public GameObject prefab;
    public float weight;
    public float coolDown;
    public float creditCost;

}
