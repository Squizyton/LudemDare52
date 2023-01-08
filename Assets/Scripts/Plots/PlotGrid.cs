using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plots;
using UnityEngine;

// A script to define the farm plots
public class PlotGrid : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private PlotCell cellPrefab;
    [SerializeField] private PlayerInventory playerInventory;
    private int[,] array;
    private List<GameObject> cells;
    

    [SerializeField] private Vector3 offset;
    // Start is called before the first frame update
    void Awake()
    {
        cells = new();
        GenerateCells();
    }


    [ContextMenu("Generate")]
    void GenerateCells()
    {
        array = new int[width, height];

        foreach (var cell in cells.Where(cell => cell))
        {
            Destroy(cell);
        }

        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
            

                cells.Add(Instantiate(cellPrefab, new Vector3(i * cellSize, 0, j * cellSize),
                    Quaternion.AngleAxis(90, new Vector3(1, 0, 0)),transform).gameObject);

                //Apply offset to the cell
                cells.Last().transform.position += offset;
            }
        }
    }
    
    public void TurnOffCellUI()
    {
        foreach (var cell in cells)
        {
            cell.GetComponent<PlotCell>().ImageStatus(false);
        }
    }
    
    public void TurnOnCellUI()
    {
        foreach (var cell in cells)
        {
            cell.GetComponent<PlotCell>().ImageStatus(true);
        }
    }
    
}