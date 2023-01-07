using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlotCell : MonoBehaviour
{
    private int value;
    private bool isMouseHovering;
    private float timeElapsed;
    private float timeToGrow;
    [SerializeField] private TextMeshPro mesh;
    private GameObject plantModel;

    public void plant(PlantInfo plant)
    {
        plantModel = plant.plantModel;
        timeToGrow = plant.GrowTime;
        value = 1;
    }

    private void Update()
    {
        if(value != 1)
        {
            return;
        }
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
        mesh.color = Color.white;
    }

    private void OnMouseDown()
    {
        if(!isMouseHovering)
        {
            return;
        }
        mesh.color = Color.yellow;
    }

    private void OnMouseUp()
    {
        if(!isMouseHovering)
        {
            return;
        }
        mesh.color = Color.white;
        value++;
        mesh.text = value.ToString();
    }
}
