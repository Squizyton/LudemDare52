using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlotCell : MonoBehaviour
{
    private int value; //Numerical display (replace with enum when I stop being dumb)

    private bool isMouseHovering; //Bools for UI state
    private bool isSelected;
    private bool isPlayerNear;

    private float timeElapsed; //Growth stuff/models
    [SerializeField] private TextMeshPro mesh;
    [SerializeField] private PlantInfo plantInfo;
    [SerializeField] private GameObject player;
    private PlayerInventory playerInventory;
    private GameObject plantModel;

    public void plant(PlantInfo plant)
    {
        plantInfo = plant;
        value = 1;
        plantModel = Instantiate(plantInfo.plantModel, this.transform.position, Quaternion.identity);
    }

    public PlantInfo harvestSeeds()
    {
        if(value != 2) return null;
        value = 0;
        mesh.text = value.ToString();
        mesh.color = Color.white;
        Destroy(plantModel);
        Debug.Log("returning!");
        Debug.Log(plantInfo.ToString());
        return plantInfo;
    }

    public void harvestAmmo() //NOTE: Make me return plantinfo when ready, and compile it into bulletstuff
    {
        if (value == 2)
        {
            value = 0;
            mesh.text = value.ToString();
            mesh.color = Color.white;
            Destroy(plantModel);
        }
    }

    private void Awake()
    {
        player.TryGetComponent(typeof(PlayerInventory), out Component inventory);
        if(inventory)
        {
            playerInventory = (PlayerInventory)inventory;
        }
    }

    private void Update()
    {
        // Only grows in state 1
        if(value != 1) return;
        // Growth
        timeElapsed += Time.deltaTime;
        if(timeElapsed > plantInfo.GrowTime)
        {
            timeElapsed = 0;
            value = 2;
            if (isPlayerNear)
            {
                mesh.color = Color.green;
            }
            mesh.text = value.ToString();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Bean" && value == 2)
        {
            mesh.color = Color.green;
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.name == "Bean" && value == 2)
        {
            mesh.color = Color.white;
            isPlayerNear = false;
        }
    }

    private void OnMouseEnter()
    {
        isMouseHovering = true;
        if (value != 0) return;
        mesh.color = Color.red;
    }

    private void OnMouseExit()
    {
        isMouseHovering = false;
        isSelected = false;
        mesh.color = Color.white;
    }

    private void OnMouseDown()
    {
        if (!isMouseHovering) return;
        isSelected = true;
        mesh.color = Color.yellow;
    }

    private void OnMouseUp()
    {
        if (!isSelected) return;

        if(value == 0)
        {
            Debug.Log("WE PLANTIN");
            Debug.Log(plantInfo.ToString());
            if (playerInventory.RemoveSeed(plantInfo))
            {
                Debug.Log("THE SEED IS REMOVED. IT IS DONE.");
                plant(plantInfo);
                mesh.color = Color.white;
            }
        }
        mesh.text = value.ToString();
    }
}
