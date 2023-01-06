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
    [SerializeField] private float timeToGrow = 1.5f;
    [SerializeField] private TextMeshPro mesh;



    private void Update()
    {
        if(value != 0)
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
