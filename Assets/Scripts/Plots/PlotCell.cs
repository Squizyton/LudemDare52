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
    private float timeToGrow;
    [SerializeField] private TextMeshPro mesh;
    [SerializeField] private PlantInfo CornInfo;
    private GameObject plantModel;

    public void plant(PlantInfo plant)
    {
        timeToGrow = plant.GrowTime;
        value = 1;
        plantModel = Instantiate(plant.plantModel, this.transform.position, Quaternion.identity);
    }

    public void harvestSeeds()
    {
        if(value == 2)
        {
            value = 0;
            mesh.text = value.ToString();
            mesh.color = Color.white;
            Destroy(plantModel);
        }
    }

    public void harvestAmmo()
    {
        if (value == 2)
        {
            value = 0;
            mesh.text = value.ToString();
            mesh.color = Color.white;
            Destroy(plantModel);
        }
    }

    private void Update()
    {
        // Check if player is near and harvesting
        if(value == 2 && isPlayerNear)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                harvestAmmo();
            }
        }
        // Only grows in state 1
        if(value != 1)
        {
            return;
        }
        // Growth
        timeElapsed += Time.deltaTime;
        if(timeElapsed > timeToGrow)
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
        if (!isMouseHovering)
        {
            return;
        }
        isSelected = true;
        mesh.color = Color.yellow;
    }

    private void OnMouseUp()
    {
        if (!isSelected)
        {
            return;
        }
        if(value == 0)
        {
            plant(CornInfo);
            mesh.color = Color.white;
        }
        else if(value == 2)
        {
            harvestSeeds();
            mesh.color = Color.red;
        }
        mesh.text = value.ToString();
    }
}
