using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMOD;

//Based upon Natty Creations "Dynamic Footsteps!: For Terrains and Imported Meshes (Unity3D)"

public class TerrainTextureFinder : MonoBehaviour 
{
    private string currentLayer;
    private string fmodLayer;

    public string CheckLayers(Vector3 playerPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3))
        {
            if (hit.transform.GetComponent<Terrain>() != null)
            {
                Terrain t = hit.transform.GetComponent<Terrain>();
                if (currentLayer != GetLayerName(transform.position, t))
                {
                    currentLayer = GetLayerName(transform.position, t);

                    ChangeFootstepLayer();
                    return fmodLayer;
                }
            }
        }
        ChangeFootstepLayer();
        return fmodLayer;
    }

    private string GetLayerName(Vector3 playerPos, Terrain t)
    {
        float[] cellMix = GetTextureMix(playerPos, t);
        float strongest = 0;
        int maxIndex = 0;
        for (int i = 0; i<cellMix.Length; i++)
        {
            if (cellMix[i] > strongest)
            {
                maxIndex = i;
                strongest = cellMix[i];
            }
        }
        return t.terrainData.terrainLayers[maxIndex].name;
    }
    private float[] GetTextureMix(Vector3 playerPos, Terrain t)
    {
        Vector3 tPos = t.transform.position;
        TerrainData tData = t.terrainData;
        int mapX = Mathf.RoundToInt((playerPos.x - tPos.x) / tData.size.x * tData.alphamapWidth);
        int mapZ = Mathf.RoundToInt((playerPos.z - tPos.z) / tData.size.z * tData.alphamapHeight);

        float[,,] splatMapData = tData.GetAlphamaps(mapX, mapZ, 1, 1);

        float[] cellmix = new float[splatMapData.GetUpperBound(2) + 1];
        for (int i = 0; i < cellmix.Length; i++)
        {
            cellmix[i] = splatMapData[0, 0, i];
        }
        return cellmix;
    }


    private void ChangeFootstepLayer()
    {

        switch (currentLayer)
        {
            case "FAE_Dirt":
                {
                    fmodLayer = "Dirt";
                    break;
                }
            case "FAE_Grass":
                {
                    fmodLayer = "Grass";
                    break;
                }
            default:
                {
                    fmodLayer = "Default";
                    break;
                }

        }
    }
}
