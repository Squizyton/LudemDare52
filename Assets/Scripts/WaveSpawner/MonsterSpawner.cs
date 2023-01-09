using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    [Header("Credits")]
    public float availableCredits;
    
    [Header("Monsters")]
    [SerializeField]private List<MonsterObject> monsterList;

    [Header("Spawn Settings")]
    [SerializeField] private List<Transform> spawnPoints;


    private bool _isSpawning;
    private int attemptsToSpawn = 0;
    private void Start()
    {
        
    }

    public void StartWave()
    {
        attemptsToSpawn = 0;
        _isSpawning = true;
        SpawnEnemy();
    }

    public bool IsSpawning()
    {
        return _isSpawning;
    }
    

    private void SpawnEnemy()
    {
        
        //Get a monster
        var pickedEnemy = GetMonster();

        //Ask to see if we can afford it
        if (PayCredits(pickedEnemy, true))
        {
            //if we can afford it, spawn it
            GameManager.Instance.enemiesRemaining++;
            UIManager.Instance.UpdateEnemiesRemaining(GameManager.Instance.enemiesRemaining);

            //Declare a spawn Point
            var index = Random.Range(0, spawnPoints.Count);
            
            //Instantiate the monster
            var enemy = Instantiate(pickedEnemy.prefab, spawnPoints[index].position, pickedEnemy.prefab.transform.rotation);

            //Start spawn cooldown
            if(gameObject.activeSelf)
                StartCoroutine(Cooldown(pickedEnemy.coolDown));
        }
        else
        {
            attemptsToSpawn++;

            if (attemptsToSpawn < 4)
            {
                //If we couldn't afford it, start cooldown
                StartCoroutine(Cooldown(.1f));
            }else _isSpawning = false;
        }
    }




    private MonsterObject GetMonster()
    {
        
        //make a temporary list of monsters
        var tempList = monsterList;
        
        //shuffle the list
        tempList = tempList.OrderBy(x => Random.value).ToList();

        var weightSum = 0f;

        var weightIndex = new List<float>();
        
        
        //loop through the collection of available monsters and grab each weight
        foreach (var monster in tempList.Where(monster => monster.weight > 0))
        {
            weightSum += monster.weight;
            weightIndex.Add(weightSum);
        }

        var index = 0;
        var lastIndex = monsterList.Count - 1;

        while (index < lastIndex)
        {
            //Do a probability check with a likelihood of weight. The greater the number, the greater the more likely its to spawn
            if (Random.Range(0, weightSum) < weightIndex[index])
            {
                return tempList[index];
            }
            
            //Remove the last item from the sum of total untested weights and try again
            weightSum -= weightIndex[index];

            index++;
        }

        return tempList[index];
    }

    private bool PayCredits(MonsterObject mon, bool actuallyBuy)
    {
        if (availableCredits < mon.creditCost) return false;
        
        if(actuallyBuy)
            availableCredits -= mon.creditCost;

        return true;
    }
    
    private IEnumerator Cooldown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SpawnEnemy();
    }
}
