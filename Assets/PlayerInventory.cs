using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    
    /// TODO: Switch these from GameObject's to Gun Class when ready
    [SerializeField] public GameObject currentActiveGun;
    [SerializeField] private GameObject[] guns;
    [SerializeField] private int currentGun;

    
    
    public void Start()
    {
        //Set the Peashooter to the first gun
        currentActiveGun = guns[0];
    }

    public void OnWeaponSwapButton(InputAction.CallbackContext)
    {
            
    }

    private void WeaponSwap()
    {
        
    }



}
