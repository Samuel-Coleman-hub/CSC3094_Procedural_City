using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class CityManager : MonoBehaviour
{
    public bool test = false;
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

    //[Header("Building Settings")]
    [SerializeField] public BuildingGenerator buildingGenerator;
    //[SerializeField] public GameObject buildingPrefab;
    
    //[SerializeField] private int CityCentreRadius;
    //[SerializeField] private int chanceOfBuilding;
    //[SerializeField] List<Material> materials = new List<Material>();
    //private float randomPercentage = 0.5f;

    [Header("Road Settings")]
    [SerializeField] public RoadVisualizer visualizer;
    [SerializeField] public Material pavementMaterial;
    

    public GridTile[,] gridMatrix;

    [Header("City Zones")]
    [SerializeField] public List<CityZone> zones = new List<CityZone>();

    [HideInInspector]
    public Vector3 centerOfZones;

    private void Start()
    {
        if (test)
        {
            TestStart();
        }
        else
        {
            NormalStart();
        }
    }

    private void TestStart()
    {
        GenerateGrid();
        //for (int i = 0; i < zones.Count; i++)
        //{
        //    if (zones[i].generateRoadsForZone)
        //    {
        //        zones[i].roadEndToCenter = visualizer.GenerateRoad(zones[i].zoneCenter, zones[i]);
        //    }
        //}
        //if (zones.Count > 1)
        //{
        //    visualizer.ConnectZones(zones);
        //}



        //visualizer.roadHelper.transform.position = new Vector3(visualizer.roadHelper.transform.position.x,
        //    gridCenter.y - 0.15f, visualizer.roadHelper.transform.position.z);

        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        List<GridTile> tiles = new List<GridTile>();
        tiles.Add(gridMatrix[0, 0]);
        tiles.Add(gridMatrix[0, 1]);
        tiles.Add(gridMatrix[1,0]);
        tiles.Add(gridMatrix[1, 1]);
        tiles.Add(gridMatrix[1, 2]);
        tiles.Add(gridMatrix[2, 1]);
        tiles.Add(gridMatrix[2, 2]);
        tiles.Add(gridMatrix[0, 2]);
        tiles.Add(gridMatrix[2, 0]);
        tiles.Add(gridMatrix[3, 0]);
        tiles.Add(gridMatrix[3, 1]);
        tiles.Add(gridMatrix[3, 2]);
        tiles.Add(gridMatrix[3, 3]);
        tiles.Add(gridMatrix[4, 3]);
        tiles.Add(gridMatrix[3, 4]);
        tiles.Add(gridMatrix[4, 4]);
        tiles.Add(gridMatrix[5, 3]);
        tiles.Add(gridMatrix[3, 5]);
        tiles.Add(gridMatrix[4, 5]);
        tiles.Add(gridMatrix[5, 4]);
        tiles.Add(gridMatrix[6, 5]);
        tiles.Add(gridMatrix[5, 5]);
        BuildingProducer producer = buildingGenerator.GetComponent<BuildingProducer>();


        stopwatch.Start();

        producer.Build(tiles, NearRoadDirection.South, 20, 1);

        stopwatch.Stop();
        TimeSpan timeSpan = stopwatch.Elapsed;
        Debug.Log(timeSpan.ToString(@"m\:ss\.ffff"));
    }

    private void NormalStart()
    {
        //StartCoroutine(TestStart());
        GenerateGrid();

        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i].generateRoadsForZone)
            {
                zones[i].roadEndToCenter = visualizer.GenerateRoad(zones[i].zoneCenter, zones[i]);
            }
        }
        if (zones.Count > 1)
        {
            visualizer.ConnectZones(zones);
        }

        visualizer.roadHelper.transform.position = new Vector3(visualizer.roadHelper.transform.position.x,
            gridCenter.y - 0.15f, visualizer.roadHelper.transform.position.z);

        buildingGenerator.GenerateBuildings(100, gridMatrix);

        gridSpawner.SpawnMiscellaneous();
    }

    public void GenerateBuildings()
    {
        buildingGenerator.GenerateBuildings(50, gridMatrix);
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
}

[System.Serializable]
public class CityZone
{
    public string zoneName;
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

    [Range(1, 10)]
    public int minBuildingHeight;
    [Range(1, 10)]
    public int maxBuildingHeight;

    [Range(1, 6)]
    public int minBuildingWidth;
    [Range(1, 6)]
    public int maxBuildingWidth;
    [Range(1f, 10f)]
    public float buildingHeightScaleMultiplier;
    [Range(1f, 6f)]
    public float buildingWidthScaleMultiplier;
    public AnimationCurve heightCenterCurve;
    public AnimationCurve widthCenterCurve;

    public List<Material> buildingMainColourMaterials;

    [Header("Tree Settings")]
    public bool placeTrees;
    [Range(1, 10)]
    public float treeDensity;
    public List<GameObject> treePrefabs;

    [Header("Lamp Post Settings")]
    public bool placeLampPosts;
    [Range(1, 10)]
    public float lampPostDensity;
    public List<GameObject> lampPostPrefabs;

    //GameObject is the prefab int is the multipliers
    [Tooltip("The Prefabs of misc objects")]
    public List<GameObject> miscObjects;
    [Tooltip("The density of the previous objects")]
    [Range(1, 30)]
    public List<float> miscDensity;

    [HideInInspector]
    public List<Vector2> positionsInZone;
    [HideInInspector]
    public Vector3 roadEndToCenter;
    [HideInInspector]
    public Vector3 zoneCenter;
    
}
