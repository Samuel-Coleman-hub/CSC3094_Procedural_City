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
    [SerializeField] public int numCityZones;
    [SerializeField] public float gridSpacing = 0f;
    [SerializeField] public Vector3 gridOrigin = Vector3.zero;
    [SerializeField] public GameObject gridTilePrefab;
    [SerializeField] public Vector3 gridCenter;


    

    [Header("Building Settings")]
    [SerializeField] public BuildingGenerator buildingGenerator;
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
        Dictionary<Zone, Vector2Int> zoneCentroids = gridSpawner.ZoneCentroids();
        Vector3 industrialCenter = new Vector3(zoneCentroids[Zone.Industrial].x, 0f, zoneCentroids[Zone.Industrial].y);
        Vector3 residentalCenter = new Vector3(zoneCentroids[Zone.Residential].x, 0f, zoneCentroids[Zone.Residential].y);
        Vector3 agricultureCenter = new Vector3(zoneCentroids[Zone.Agriculture].x, 0f, zoneCentroids[Zone.Agriculture].y);
        visualizer.StartRoadGeneration(industrialCenter, 6);
        visualizer.StartRoadGeneration(residentalCenter, 6);
        visualizer.StartRoadGeneration(agricultureCenter, 10);
        visualizer.roadHelper.transform.position = new Vector3(visualizer.roadHelper.transform.position.x,
            gridCenter.y - 0.15f, visualizer.roadHelper.transform.position.z);
    }

    public void GenerateBuildings()
    {
        buildingGenerator.GenerateBuildings(materials, buildingPrefab, randomPercentage, gridMatrix);
    }

    public void GenerateGrid()
    {
        gridCenter = new Vector3(x / 2, 0.45f, z / 2);
        gridMatrix = gridSpawner.GenerateGrid(x, z, gridSpacing, numCityZones, gridOrigin, gridTilePrefab);
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
