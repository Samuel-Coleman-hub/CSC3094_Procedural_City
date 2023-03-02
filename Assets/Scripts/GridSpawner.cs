using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridSpawner : MonoBehaviour
{
    //Grid Generation Variables
    private int x;
    private int z;
    private float gridSpacing;
    private Vector3 gridOrigin;
    private GameObject gridTilePrefab;
    private GridTile[,] gridMatrix;

    // Start is called before the first frame update
    private GridTile[,] SpawnGrid()
    {
        gridMatrix = new GridTile[x, z];

        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < z; j++)
            {
                Vector3 spawnPos = new Vector3(i * gridSpacing, 0, j* gridSpacing) + gridOrigin;
                GameObject temp = Instantiate(gridTilePrefab, spawnPos, Quaternion.identity, transform);
                //double centerScore = Math.Sqrt((Math.Pow(i - j, 2) + Math.Pow((x/2) - (z/2), 2)));
                double centerScore = Math.Floor(Math.Sqrt(Math.Pow(((x/2)-i), 2) + Math.Pow(((z/2)-j), 2)));
                gridMatrix[i, j] = new GridTile(temp, i, j, centerScore, TileType.Empty);
            }
        }

        return gridMatrix;
        
    }

    public GridTile[,] GenerateGrid(int x, int z, float gridSpacing, Vector3 gridOrigin, GameObject gridTilePrefab)
    {
        this.x = x;
        this.z = z;
        this.gridSpacing = gridSpacing;
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

}
