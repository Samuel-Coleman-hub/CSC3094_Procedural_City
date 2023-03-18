using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GridSpawner : MonoBehaviour
{
    //Grid Generation Variables
    private int x;
    private int z;
    private float gridSpacing;
    private Vector3 gridOrigin;
    private GameObject gridTilePrefab;
    private GridTile[,] gridMatrix;
    private int numCityZones;

    private Vector2Int[] centroids;
    private Color[] colourRegions;

    // Start is called before the first frame update
    private GridTile[,] SpawnGrid()
    {
        gridMatrix = new GridTile[x, z];

        StartVoroni();
        Color[] pixelColors = new Color[x * z];

        for (int i = 0; i < x; i++)
        {
            for(int j = 0; j < z; j++)
            {
                Vector3 spawnPos = new Vector3(i * gridSpacing, 0, j* gridSpacing) + gridOrigin;
                GameObject temp = Instantiate(gridTilePrefab, spawnPos, Quaternion.identity, transform);
                double centerScore = Math.Floor(Math.Sqrt(Math.Pow(((x/2)-i), 2) + Math.Pow(((z/2)-j), 2)));
                gridMatrix[i, j] = new GridTile(temp, i, j, centerScore, TileType.Empty);

                //For Voronoi Colours
                int closestCentroidIndex = GetClosestCentroidIndex(new Vector2Int(i, j), centroids);
                gridMatrix[i, j].Object.GetComponent<MeshRenderer>().material.color = colourRegions[closestCentroidIndex];

                //Maybe change this so that it works with any number of zones
                switch (closestCentroidIndex)
                {
                    case 0:
                        gridMatrix[i, j].Zone = Zone.Industrial;
                        break;
                    case 1:
                        gridMatrix[i, j].Zone = Zone.Residential;
                        break;
                    case 2:
                        gridMatrix[i, j].Zone = Zone.Agriculture;
                        break;
                }
                
                
                //For debugging
                temp.GetComponentInChildren<TextMeshProUGUI>().text = "Row " + i + " , Column " + j + " " + gridMatrix[i,j].Zone; 
            }
        }

        return gridMatrix;
    }

    public GridTile[,] GenerateGrid(int x, int z, float gridSpacing, int numCityZones, Vector3 gridOrigin, GameObject gridTilePrefab)
    {
        this.x = x;
        this.z = z;
        this.gridSpacing = gridSpacing;
        this.numCityZones = numCityZones;
        this.gridOrigin = gridOrigin;
        this.gridTilePrefab = gridTilePrefab;


        if (gridMatrix != null)
        {
            for (int i = 0; i < gridMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < gridMatrix.GetLength(1); j++)
                {
                    Destroy(gridMatrix[i, j].ChildObject);
                    Destroy(gridMatrix[i, j].Object);
                    gridMatrix[i, j] = null;
                }
            }
        }

        gridMatrix = null;

        return SpawnGrid();
    }

    private void StartVoroni()
    {
        centroids = new Vector2Int[numCityZones];
        colourRegions = new Color[numCityZones];
        for (int i = 0; i < numCityZones; i++)
        {
            centroids[i] = new Vector2Int(Random.Range(0, x), Random.Range(0, z));
            colourRegions[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
    }

    private int GetClosestCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroids)
    {
        float closestDst = float.MaxValue;
        int index = 0;
        for (int i = 0; i < centroids.Length; i++)
        {
            if (Vector2.Distance(pixelPos, centroids[i]) < closestDst)
            {
                closestDst = Vector2.Distance(pixelPos, centroids[i]);
                index = i;
            }
        }
        return index;
    }

}
