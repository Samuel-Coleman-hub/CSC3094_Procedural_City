using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityManager : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] public Slider xSlider;
    [SerializeField] public Slider zSlider;
    [SerializeField] public Slider randomSlider;

    [Header("Grid Setttings")]
    [SerializeField] public GridSpawner gridSpawner;
    [SerializeField] public int x;
    [SerializeField] public int z;
    [SerializeField] public float gridSpacing = 0f;
    [SerializeField] public Vector3 gridOrigin = Vector3.zero;
    [SerializeField] public GameObject gridTilePrefab;
    [SerializeField] public Vector3 gridCenter;

    [Header("Building Settings")]
    [SerializeField] public GameObject buildingPrefab;
    [SerializeField] private int CityCentreRadius;
    [SerializeField] private int chanceOfBuilding;
    [SerializeField] List<Material> materials = new List<Material>();
    private float randomPercentage = 0.5f;

    [Header("Road Settings")]
    [SerializeField] public RoadVisualizer visualizer;
    

    public GridTile[,] gridMatrix;
    

    private void Start()
    {
        GenerateGrid();
        visualizer.StartRoadGeneration();
    }

    public void GenerateBuildings()
    {
        gridSpawner.GenerateBuildings(materials, buildingPrefab, randomPercentage);
    }

    public void GenerateGrid()
    {
        gridCenter = new Vector3(x / 2, 0.45f, z / 2);
        gridMatrix = gridSpawner.GenerateGrid(x, z, gridSpacing, gridOrigin, gridTilePrefab);
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

    

}
