using System.Collections;
using System.Collections.Generic;
using Plots;
using UnityEngine;

// A script to define the farm plots
public class PlotGrid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private PlotCell textMeshPrefab;
    [SerializeField] private PlayerInventory playerInventory;
    private int[,] array;
    // Start is called before the first frame update
    void Start()
    {
        array = new int[width,height];
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Instantiate(textMeshPrefab, new Vector3(i*cellSize, 0, j*cellSize), Quaternion.AngleAxis(90,new Vector3(1,0,0)));
            }
        }
    }
}
