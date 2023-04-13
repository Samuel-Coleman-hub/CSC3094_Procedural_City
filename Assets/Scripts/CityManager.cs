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
    [SerializeField] public BuildingGenerator buildingGenerator;
    [SerializeField] public GameObject buildingPrefab;
    [SerializeField] private int CityCentreRadius;
    [SerializeField] private int chanceOfBuilding;
    [SerializeField] List<Material> materials = new List<Material>();
    private float randomPercentage = 0.5f;

    [Header("Road Settings")]
    [SerializeField] public RoadVisualizer visualizer;
    

    public GridTile[,] gridMatrix;

    [Header("City Zones")]
    [SerializeField] public List<CityZone> zones = new List<CityZone>();
    

    private void Start()
    {
        GenerateGrid();

        for(int i = 0; i < zones.Count; i++)
        {
            if (zones[i].generateRoadsForZone)
            {
                visualizer.StartRoadGeneration(zones[i].zoneCenter, zones[i]);
            }   
        }

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
        gridMatrix = gridSpawner.GenerateGrid(x, z, gridSpacing, zones, gridOrigin, gridTilePrefab);
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

[System.Serializable]
public class CityZone
{
    [Header("L-System Settings")]
    public bool generateRoadsForZone;
    public int roadLength;
    public Rule[] rules;
    public string axiom;
    [Range(0, 10)]
    public int iterationLimit = 1;
    public bool randomIgnoreRuleModifier = true;
    [Range(0, 1)]
    public float chanceToIgnoreRule = 0.3f;

    [Header("Building Settings")]
    [Range(0f, 3f)]
    public float chanceOfBuildingPlacement;
    [Range(3f, 10f)]
    public float buildingYScaleMultiplier;

    [HideInInspector]
    public List<Vector2> positionsInZone;
    public Vector3 zoneCenter;
    
}
