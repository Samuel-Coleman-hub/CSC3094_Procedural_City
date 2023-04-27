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

    private List<CityZone> cityZones = new List<CityZone>();
    private VoronoiDiagram voronoi;

    private CityManager cityManager;

    private GridTile[,] SpawnGrid()
    {
        cityManager = GameObject.FindGameObjectWithTag("CityManager").GetComponent<CityManager>();

        gridMatrix = new GridTile[x, z];

        voronoi = new VoronoiDiagram();
        voronoi.GenerateVoronoi(cityZones.Count, x, z);

        Color[] pixelColors = new Color[x * z];

        for (int i = 0; i < x; i++)
        {
            for(int j = 0; j < z; j++)
            {
                Vector3 spawnPos = new Vector3(i * gridSpacing, 0, j* gridSpacing) + gridOrigin;
                GameObject temp = Instantiate(gridTilePrefab, spawnPos, Quaternion.identity, transform);
                gridMatrix[i, j] = new GridTile(temp, i, j, TileType.Empty);

                int closestCentroidIndex = voronoi.GetClosestCentroidIndex(new Vector2Int(i, j));

                gridMatrix[i, j].Zone = cityZones[closestCentroidIndex];
                cityZones[closestCentroidIndex].positionsInZone.Add(new Vector2(i,j));

                //For debugging
                gridMatrix[i, j].Object.GetComponent<MeshRenderer>().material.color = voronoi.colourRegions[closestCentroidIndex];
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
