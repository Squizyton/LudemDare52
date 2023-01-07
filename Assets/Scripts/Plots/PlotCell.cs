using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlotCell : MonoBehaviour
{
    private int value;
    private bool isMouseHovering;
    private bool isSelected;
    private float timeElapsed;
    private float timeToGrow;
    [SerializeField] private TextMeshPro mesh;
    [SerializeField] private PlantInfo CornInfo;
    private GameObject plantModel;
    private GameObject plantModelInstance;

    public void plant(PlantInfo plant)
    {
        plantModel = plant.plantModel;
        timeToGrow = plant.GrowTime;
        value = 1;
        plantModelInstance = Instantiate(plantModel, this.transform.position, Quaternion.identity);
    }

    public void harvestSeeds()
    {
        if(value == 2)
        {
            value = 0;
            Destroy(plantModelInstance);
        }
    }

    private void Update()
    {
        if(value != 1)
        {
            return;
        }
        mesh.color = Color.white;
        timeElapsed += Time.deltaTime;
        if(timeElapsed > timeToGrow)
        {
            timeElapsed = 0;
            value = 2;
            mesh.text = value.ToString();
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
