using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Guns;
using Plots;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour,IHasHealth
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
    [SerializeField] PlantInfo PepperSeed;
    [SerializeField] PlantInfo CarrotSeed;
    [SerializeField] PlantInfo MelonSeed;

    //Inventory for seeds
    [SerializeField]Dictionary<PlantInfo, int> seedInventory = new Dictionary<PlantInfo, int>();
    [SerializeField]Dictionary<PlantInfo, int> bulletInventory = new Dictionary<PlantInfo, int>();
    
    private PlayerControls controls;
    
    
    /// TODO: Switch these from GameObject's to Gun Class when ready
    [SerializeField] public BaseGun currentActiveGun;
    [SerializeField] private GameObject[] guns;
    [SerializeField] private int currentGun;

    private PlotCell selectedPlot;
    public PlantInfo SelectedSeed { get; set; }

    
    private int health = 100;
    public void Start()
    {
        controls = new PlayerControls();
        controls.Player.WeaponSwapping.performed += ctx => WeaponSwap();
        controls.Player.ConvertToBullets.performed += ctx => HarvestAmmo();
        controls.Player.Harvest.performed += ctx => HarvestSeeds();
        //Set the Peashooter to the first gun


        seedInventory = new Dictionary<PlantInfo, int>();
        controls.Enable();
    }

    public int GetAmmo(PlantInfo seed)
    {
        if (bulletInventory.ContainsKey(seed))
        {
            return bulletInventory[seed];
        }
        return 0;
    }
    public void TakeDamage(int damage)
    {
        Debug.Log("taking Damage");
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //Lose Screen here
    }

    private void WeaponSwap()
    {
       
        if(GameManager.Instance.currentMode == GameManager.CurrentMode.TopDown) return;
        
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
        
        PlantInfo harvested = selectedPlot.HarvestSeeds();
        if (!harvested) return;
        AddSeed(bulletInventory, harvested, harvested.bulletYield);
        selectedPlot = null;
    }

    private void HarvestSeeds()
    {
        if(selectedPlot)
        {
            PlantInfo harvested = selectedPlot.HarvestSeeds();
            if (!harvested) return;
            AddSeed(seedInventory, harvested, harvested.seedYield);
            selectedPlot = null;
        }
    }

    public void SetSeed(string seedName)
    {
        PlantInfo seedToSelect = seedInventory.FirstOrDefault(i => (i.Key.PlantName == seedName)).Key;
        if (seedToSelect) SelectedSeed = seedToSelect;
    }

    public void AddSeed(Dictionary<PlantInfo, int> inventory, PlantInfo seed,int amount = 1)
    {
        
        
        
        if (inventory.ContainsKey(seed))
        {
            inventory[seed] += amount;
        }
        else
        {
            inventory.Add(seed, amount);
            Debug.Log("Seed Dictionary Count: " + inventory.Count);
        }
        Debug.Log(inventory[seed]);
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

    ///<summary>
    /// We make this a bool so we can check if we have enough seeds to plant/use if it doesn, return false
    /// </summary>
    public bool RemoveAmmo(PlantInfo seed, int amount = 1)
    {
        if (bulletInventory.ContainsKey(seed) && bulletInventory[seed] >= amount)
        {
            bulletInventory[seed] -= amount;
            if (bulletInventory[seed] <= 0)
            {
                bulletInventory.Remove(seed);
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
      AddSeed(seedInventory, CornSeed,19);
      AddSeed(seedInventory, PepperSeed, 2);
      AddSeed(seedInventory, CarrotSeed, 5);
      AddSeed(seedInventory, MelonSeed, 5);
    }
    [ContextMenu("Add Test Bullets")]
    public void AddTestBullets()
    {
        AddSeed(bulletInventory, CornSeed, 200);
        AddSeed(bulletInventory, PepperSeed, 200);
        AddSeed(bulletInventory, CarrotSeed, 200);
        AddSeed(bulletInventory, MelonSeed, 200);
    }
    [ContextMenu("Check Dictionary")]
    public void CheckDictionarySize()
    {
        Debug.Log("Dictionary Size: " +seedInventory.Count);
    }
    [ContextMenu("Check Peppercount")]
    public void CheckPepperCount()
    {
        Debug.Log("Pepper count: " + seedInventory[PepperSeed]);
    }
    [ContextMenu("Check Corncount")]
    public void CheckCornCount()
    {
        Debug.Log("Corn count: " + seedInventory[CornSeed]);
    }
}