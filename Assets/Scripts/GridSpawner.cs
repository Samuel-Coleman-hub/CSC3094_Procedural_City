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

    private CityManager cityManager;

    private GridTile[,] SpawnGrid()
    {
        cityManager = GameObject.FindGameObjectWithTag("CityManager").GetComponent<CityManager>();

        gridMatrix = new GridTile[x, z];

        StartVoroni();
        Color[] pixelColors = new Color[x * z];

        for (int i = 0; i < x; i++)
        {
            for(int j = 0; j < z; j++)
            {
                Vector3 spawnPos = new Vector3(i * gridSpacing, 0, j* gridSpacing) + gridOrigin;
                GameObject temp = Instantiate(gridTilePrefab, spawnPos, Quaternion.identity, transform);
                gridMatrix[i, j] = new GridTile(temp, i, j, TileType.Empty);

                //For Voronoi Colours
                int closestCentroidIndex = GetClosestCentroidIndex(new Vector2Int(i, j), centroids);

                //Shows Voronoi on grid
                //gridMatrix[i, j].Object.GetComponent<MeshRenderer>().material.color = colourRegions[closestCentroidIndex];

                //Maybe change this so that it works with any number of zones. Hello past me thank you i did
                gridMatrix[i, j].Zone = cityZones[closestCentroidIndex];
                cityZones[closestCentroidIndex].positionsInZone.Add(new Vector2(i,j));
                //For debugging
                //temp.GetComponentInChildren<TextMeshProUGUI>().text = "Row " + i + " , Column " + j + " " + gridMatrix[i,j].Zone; 
            }
        }

        FindZoneCenters();

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

    private void StartVoroni()
    {
        centroids = new Vector2Int[cityZones.Count];
        colourRegions = new Color[cityZones.Count];
        for (int i = 0; i < cityZones.Count; i++)
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

    private void FindZoneCenters()
    {
        Vector2 center = new Vector2();
        int count = 0;

        foreach (CityZone zone in cityZones)
        {
            foreach(Vector2 pos in zone.positionsInZone)
            {
                center += pos;
                count++;
            }

            Vector2 result = center / count;
            zone.zoneCenter = new Vector3(result.x, 0f, result.y);
            center = new Vector2();
            count = 0;
        }

        Vector3 centroid = new Vector3();

        foreach (CityZone zone in cityZones)
        {
            centroid += zone.zoneCenter;
            count++;
        }

        cityManager.centerOfZones = centroid / count;
        cityManager.zones = cityZones;
    }


    //Should this be in its own class
    public void SpawnMiscallenous()
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < z; j++)
            {
                CityZone zone = gridMatrix[i, j].Zone;
                //Need a better system for weighted randoms probably own class
                if (zone.placeTrees && gridMatrix[i, j].TileType.Equals(TileType.Empty) && Random.value * zone.treeDensity < 0.5f && zone.treePrefabs.Count != 0)
                {
                    int randomIndex = Random.Range(0, zone.treePrefabs.Count);
                    GameObject tree = GameObject.Instantiate(zone.treePrefabs[randomIndex]);
                    tree.transform.position = new Vector3(gridMatrix[i,j].GetX(), 0, gridMatrix[i,j].GetY());
                    gridMatrix[i, j].TileType = TileType.Misc;
                }

                if (zone.placeLampPosts && gridMatrix[i, j].TileType.Equals(TileType.Pavement) && Random.value * zone.lampPostDensity < 0.5f && zone.lampPostPrefabs.Count != 0)
                {
                    int randomIndex = Random.Range(0, zone.lampPostPrefabs.Count);
                    GameObject lampPost = GameObject.Instantiate(zone.lampPostPrefabs[randomIndex]);
                    lampPost.transform.position = new Vector3(gridMatrix[i, j].GetX(), 0, gridMatrix[i, j].GetY());
                }

                List<GameObject> miscList = gridMatrix[i, j].Zone.miscObjects;
                List<float> miscDensity = gridMatrix[i,j].Zone.miscDensity;

                if(miscList.Count > 0 && miscList.Count == miscDensity.Count)
                {
                    for (int k = 0; k < miscList.Count; k++)
                    {
                        Debug.Log("at point " + k + " in misc list");
                        if (gridMatrix[i, j].TileType.Equals(TileType.Empty) && Random.value * miscDensity[k] < 0.5f)
                        {
                            GameObject miscObject = GameObject.Instantiate(miscList[k]);
                            miscObject.transform.position = new Vector3(gridMatrix[i, j].GetX(), 0, gridMatrix[i, j].GetY());
                            gridMatrix[i, j].TileType = TileType.Misc;
                        }
                        else if (gridMatrix[i,j].TileType != TileType.Empty)
                        {
                            break;
                        }
                        
                    }
                }
            }
        }
    }
}
