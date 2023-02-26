using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridSpawner : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] public Slider xSlider;
    [SerializeField] public Slider zSlider;
    [SerializeField] public Slider randomSlider;

    [Header("Dimensions")]
    [SerializeField] public int x;
    [SerializeField] public int z;
    [SerializeField] public float gridSpacing = 0f;
    [SerializeField] public Vector3 gridOrigin = Vector3.zero;
    [SerializeField] public GameObject gridTilePrefab;
    [SerializeField] public GameObject buildingPrefab;

    [Header("Randomosity")]
    [SerializeField] private int CityCentreRadius;
    [SerializeField] private int chanceOfBuilding;

    [SerializeField] List<Material> materials = new List<Material>();

    private GridTile[,] gridMatrix;

    private float randomPercentage = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void SpawnGrid()
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
                Debug.Log("Center is " + (gridMatrix.GetLength(0)/2) + (gridMatrix.GetLength(1)/2));
                Debug.Log(i + " " + j + " " + centerScore);
                gridMatrix[i, j] = new GridTile(temp, i, j, centerScore, TileType.Empty);
            }
        }
        
    }

    private void SpawnBuildings()
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < z; j++)
            {
                if (gridMatrix[i, j].ChildObject != null)
                {
                    Destroy(gridMatrix[i, j].ChildObject);
                    gridMatrix[i,j].TileType = TileType.Empty;
                }

                if (gridMatrix[i, j].TileType.Equals(TileType.Empty) && UnityEngine.Random.value * gridMatrix[i,j].CenterScore <= randomPercentage * 2)
                {
                    GameObject tile = gridMatrix[i, j].Object;
                    GameObject buildingObject = Instantiate(buildingPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y+1f,
                        tile.transform.position.z), Quaternion.identity);
                    buildingObject.GetComponent<MeshRenderer>().material = materials[UnityEngine.Random.Range(0, materials.Count)];

                    gridMatrix[i, j].ChildObject = buildingObject;
                    gridMatrix[i, j].TileType = TileType.Building;
                }
            }
        }
    }

    public void SetX()
    {
        x = (int)xSlider.value;
        Debug.Log(x);
    }

    public void SetY()
    {
        z = (int)zSlider.value;
    }

    public void SetRandom()
    {
         randomPercentage = randomSlider.value;
    }

    public void GenerateGrid()
    {
        if(gridMatrix != null)
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

        SpawnGrid();
    }

    public void GenerateBuildings()
    {
        SpawnBuildings();
    }

}
