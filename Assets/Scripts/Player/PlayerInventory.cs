using System;
using System.Collections;
using System.Collections.Generic;
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
    }

    //Inventory for seeds
    [SerializeField]Dictionary<PlantInfo, int> seedInventory = new Dictionary<PlantInfo, int>();
    
    //[SerializeField]Dictionary<BulletStuff, int> bulletInventory = new Dictionary<BulletStuff, int>();
    
    PlayerControls controls;
    
    
    /// TODO: Switch these from GameObject's to Gun Class when ready
    [SerializeField] public BaseGun currentActiveGun;
    [SerializeField] private BaseGun[] guns;
    [SerializeField] private int currentGun;

    
    
    public void Start()
    {
        controls = new PlayerControls();
        controls.Player.WeaponSwapping.performed += ctx => WeaponSwap();
        //Set the Peashooter to the first gun

        controls.Enable();
    }
    private void WeaponSwap()
    {
        currentGun = (currentGun + 1) % guns.Length;
        currentActiveGun.gameObject.SetActive(false);
        currentActiveGun = guns[currentGun];
        currentActiveGun.gameObject.SetActive(true);
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
       }
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
           return true;
       }
       return false;
   }
    
}
