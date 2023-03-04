using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FlyingWormConsole3.LiteNetLib.Utils;
using Guns;
using Plots;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private FMODSoundManager fmodSoundManager;
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

        if (fmodSoundManager == null) 
            fmodSoundManager = FindObjectOfType<FMODSoundManager>();
    }
    
    
    [SerializeField] PlantInfo CornSeed;
    [SerializeField] PlantInfo PepperSeed;
    [SerializeField] PlantInfo CarrotSeed;
    [SerializeField] PlantInfo MelonSeed;
    [SerializeField] PlantInfo StarFruitSeed;
    //Inventory for seeds
    [SerializeField]public Dictionary<PlantInfo, int> seedInventory = new Dictionary<PlantInfo, int>();
    
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
        controls.Player.WeaponSwapping.performed += WeaponSwap;
        controls.Player.Harvest.performed += HarvestSeeds;
        //Set the Peashooter to the first gun
        //Add initial seeds
        seedInventory = new Dictionary<PlantInfo, int>();
        
        AddSeed(seedInventory, CornSeed, 90);
        AddSeed(seedInventory, PepperSeed, 60);
        AddSeed(seedInventory, MelonSeed, 1);
        AddSeed(seedInventory, CarrotSeed, 15);
        AddSeed(seedInventory, StarFruitSeed, 1);




        UIManager.Instance.UpdateAmmoCount(currentActiveGun.GetCurrentMag(),0,currentActiveGun.GetIsInfinite());
        controls.Enable();
        
        
    }


    public int GetAmmo(PlantInfo seed)
    {
        if (seedInventory.ContainsKey(seed))
        {
            return seedInventory[seed];
        }
        return 0;
    }

    public float GetHealth()
    {
        return health;
    }
    public void TakeDamage(Vector3 attackerPos, int damageTaken)
    {
        
 
        Vector3 direction = attackerPos - transform.position;
        UIManager.Instance.TriggerDamageIndicator(direction);
        
        health -= damageTaken;
        UIManager.Instance.UpdateHealth();
        /*
        if (fmodSoundManager != null)
            fmodSoundManager.UpdateHealth(health);
        */
        this.GetComponentInChildren<TensionControler>().UpdateHealth(health);

        if (health <= 0)
        {
            //FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Voice/Player_Voice_Death");
            Die();
        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Voice/Player_Voice_Hurt");
            this.GetComponentInChildren<TensionControler>().WasHit();
        }
        
    }

    private void Die()
    {
        GameManager.Instance.ChangeMode(GameManager.CurrentMode.GameOver);
        SceneManager.LoadSceneAsync("GameOverScreen");
    }
    
    
    private void WeaponSwap(InputAction.CallbackContext context)
    {
       
        if(GameManager.Instance.currentMode == GameManager.CurrentMode.TopDown) return;
        
        currentGun = (currentGun + 1) % guns.Length;
      
        //Disable the current gun
        currentActiveGun.AbortReloadSequence();
        if(currentActiveGun.gameObject.activeSelf)
            currentActiveGun.gameObject.SetActive(false);
    
        
        //set the new gun to the current gun
        guns[currentGun].TryGetComponent(out BaseGun gun);
        currentActiveGun = gun;
         
         
        //Enable the new gun
        guns[currentGun].SetActive(true);
        UIManager.Instance.UpdateAmmoType(currentActiveGun.bulletList[currentActiveGun.currentBullet].GetBulletInfo());
        
        if(currentActiveGun.bulletList[currentActiveGun.currentBullet].GetBulletInfo().PlantName == "pea")
            UIManager.Instance.UpdateAmmoCount(currentActiveGun.GetCurrentMag(),0,currentActiveGun.GetIsInfinite());

        currentActiveGun.ChangeFMODGunType();    //to be discontinued
    }

    private void HarvestSeeds(InputAction.CallbackContext context)
    {
        if (!selectedPlot) return;
        
        GameManager.Instance.cropsHarvested++;
        UIManager.Instance.HarvestText(false);
        PlantInfo harvested = selectedPlot.HarvestSeeds();
        selectedPlot.TurnOnGrowingInfo(false);
        
        if (!harvested) return;
        FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/UI/UI_Player_Harvest", gameObject);
        if (harvested.PlantName == "starfruit")
        {
            health += (int)harvested.bulletDamage;
            UIManager.Instance.UpdateHealth();
            return;
        }
        AddSeed(seedInventory, harvested, harvested.seedYield);
        UIManager.Instance.UpdateSeedCount(harvested);
        if (harvested == currentActiveGun.bulletList[currentActiveGun.currentBullet].GetBulletInfo())
        {
            UIManager.Instance.UpdateAmmoCount(currentActiveGun.GetCurrentMag(), seedInventory[harvested],
                currentActiveGun.GetIsInfinite());
            currentActiveGun.UpdateSack(harvested);
        }

        selectedPlot = null;
    }

    public void SetSeed(string seedName)
    {
        PlantInfo seedToSelect = seedInventory.FirstOrDefault(i => (i.Key.PlantName == seedName)).Key;
        if (seedToSelect) SelectedSeed = seedToSelect;
    }

    public int GetSeedCount(PlantInfo plantInfo)
    {
        if(seedInventory.ContainsKey(plantInfo))
            return seedInventory[plantInfo];
        else
            return 0;
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
       
        }

        UIManager.Instance.UpdateSeedCount(seed);
    }
   
   
    ///<summary>
    /// We make this a bool so we can check if we have enough seeds to plant/use if it doesn, return false
    /// </summary>
    public bool RemoveSeed(PlantInfo seed, int amount = 1)
    {
        if (seedInventory.ContainsKey(seed) && seedInventory[seed] >= amount)
        {
            seedInventory[seed] -= amount;
            if (seedInventory[seed] <= 0)
            {
                seedInventory.Remove(seed);
                
            }
            UIManager.Instance.UpdateSeedCount(seed);
            return true;
        }
        return false;
    }

    ///<summary>
    /// We make this a bool so we can check if we have enough seeds to plant/use if it doesn, return false
    /// </summary>
    public bool RemoveAmmo(PlantInfo seed, int amount = 1)
    {
        if (seedInventory.ContainsKey(seed) && seedInventory[seed] >= amount)
        {
            seedInventory[seed] -= amount;
            if (seedInventory[seed] < 0)
            {
                seedInventory.Remove(seed);
            }
            UIManager.Instance.UpdateSeedCount(seed);
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
        AddSeed(seedInventory, CornSeed, 200);
        AddSeed(seedInventory, PepperSeed, 200);
        AddSeed(seedInventory, CarrotSeed, 200);
        AddSeed(seedInventory, MelonSeed, 200);
    }
    [ContextMenu("Check Dictionary")]
    public void CheckDictionarySize()
    {
       
    }
    [ContextMenu("Check Peppercount")]
    public void CheckPepperCount()
    {
     
    }
    [ContextMenu("Check Corncount")]
    public void CheckCornCount()
    {

    }
    private void OnDestroy()
    {
        controls.Player.WeaponSwapping.performed -= WeaponSwap;
        controls.Player.Harvest.performed -= HarvestSeeds;
    }
}