using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Guns;
using Plots;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        
        seedInventory = new Dictionary<PlantInfo, int>();
    }
    [SerializeField] PlantInfo CornSeed;
    //Inventory for seeds
    [SerializeField]Dictionary<PlantInfo, int> seedInventory = new Dictionary<PlantInfo, int>();
    
    //[SerializeField]Dictionary<BulletStuff, int> bulletInventory = new Dictionary<BulletStuff, int>();
    
    private PlayerControls controls;
    
    
    /// TODO: Switch these from GameObject's to Gun Class when ready
    [SerializeField] public BaseGun currentActiveGun;
    [SerializeField] private GameObject[] guns;
    [SerializeField] private int currentGun;

    private PlotCell selectedPlot;

    public void Start()
    {
        controls = new PlayerControls();
        controls.Player.WeaponSwapping.performed += ctx => WeaponSwap();
        controls.Player.ConvertToBullets.performed += ctx => HarvestAmmo();
        controls.Player.Harvest.performed += ctx => HarvestSeeds();
        //Set the Peashooter to the first gun

        controls.Enable();
    }

    private void WeaponSwap()
    {
        Debug.Log("Gun Length" + guns.Length);
        currentGun = (currentGun + 1) % guns.Length;
        Debug.Log("Current Gun" + guns[currentGun]);
        //Disable the current gun
        
        if(currentActiveGun.gameObject.activeSelf)
            currentActiveGun.gameObject.SetActive(false);
    
        
        //set the new gun to the current gun
         guns[currentGun].TryGetComponent(out BaseGun gun);
         currentActiveGun = gun;
         
         
        //Enable the new gun
        guns[currentGun].SetActive(true);
    }

    private void HarvestAmmo()
    {
        if (!selectedPlot) return;
        
        selectedPlot.HarvestAmmo();
        selectedPlot = null;
    }

    private void HarvestSeeds()
    {
        if(selectedPlot)
        {
            PlantInfo harvested = selectedPlot.HarvestSeeds();
            if (!harvested) return;
            AddSeed(harvested);
            selectedPlot = null;
        }
        
        
        
    }
    
    public void AddSeed(PlantInfo seed,int amount = 1)
    {
        if (seedInventory.ContainsKey(seed))
        {
            seedInventory[seed] += amount;
        }
        else
        {
            seedInventory.Add(seed, amount);
            Debug.Log("Seed Dictionary Count: " +seedInventory.Count);
        }
        Debug.Log(seedInventory[seed]);
    }
   
   
    ///<summary>
    /// We make this a bool so we can check if we have enough seeds to plant/use if it doesn, return false
    /// </summary>
    public bool RemoveSeed(PlantInfo seed, int amount = 1)
    {
        Debug.Log(seedInventory.Count);
        Debug.Log("Contains Key: " + seedInventory.ContainsKey(seed));
        
        
        if (seedInventory.ContainsKey(seed) && seedInventory[seed] >= amount)
        {
            seedInventory[seed] -= amount;
            if (seedInventory[seed] <= 0)
            {
                seedInventory.Remove(seed);
            }
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(typeof(PlotCell), out Component cell))
        {
            if (cell)
            {
                selectedPlot = (PlotCell)cell;
            }
        }
    }

    [ContextMenu("Add Test Seed")]
    public void AddTestSeed()
    {
      AddSeed(CornSeed,19);
        Debug.Log(CornSeed);
        Debug.Log(seedInventory[CornSeed]);
    }
    [ContextMenu("Check Dictionary")]
    public void CheckDictionarySize()
    {
        Debug.Log("Dictionary Size: " +seedInventory.Count);
    }
}