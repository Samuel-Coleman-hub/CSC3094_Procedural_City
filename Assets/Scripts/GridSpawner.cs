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

    private Vector2Int[] centroids;
    private Color[] colourRegions;

    private List<CityZone> cityZones = new List<CityZone>();
    private List<Vector3> zoneCentroids = new List<Vector3>();

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

                //Maybe change this so that it works with any number of zones. Hello past me thank you i did
                Debug.Log("Hello my closest centroid is " + closestCentroidIndex);
                Debug.Log("this is the length of zones " + cityZones.Count);
                gridMatrix[i, j].Zone = cityZones[closestCentroidIndex];
                //For debugging
                temp.GetComponentInChildren<TextMeshProUGUI>().text = "Row " + i + " , Column " + j + " " + gridMatrix[i,j].Zone; 
            }
        }

        return gridMatrix;
    }

    public GridTile[,] GenerateGrid(int x, int z, float gridSpacing, List<CityZone> cityZones, Vector3 gridOrigin, GameObject gridTilePrefab)
    {
        this.x = x;
        this.z = z;
        this.gridSpacing = gridSpacing;
        this.cityZones = cityZones;
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

    public List<Vector3> ZoneCentroids()
    {
        return zoneCentroids;
    }

    private void StartVoroni()
    {
        centroids = new Vector2Int[cityZones.Count];
        colourRegions = new Color[cityZones.Count];
        for (int i = 0; i < cityZones.Count; i++)
        {
            centroids[i] = new Vector2Int(Random.Range(0, x), Random.Range(0, z));
            colourRegions[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            zoneCentroids.Add(new Vector3(centroids[i].x, 0f, centroids[i].y));
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
