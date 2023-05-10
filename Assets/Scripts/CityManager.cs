using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class CityManager : MonoBehaviour
{
    [Header("Grid Setttings")]
    [SerializeField] private GridSpawner gridSpawner;
    [SerializeField] private int x;
    [SerializeField] private int z;
    [SerializeField] private float gridSpacing = 0f;
    [SerializeField] private Vector3 gridOrigin = Vector3.zero;
    [SerializeField] private GameObject gridTilePrefab;
    [SerializeField] private Vector3 gridCenter;

    //[Header("Building Settings")]
    [SerializeField] private BuildingGenerator buildingGenerator;


    [Header("Road Settings")]
    [SerializeField] private RoadVisualizer visualizer;
    [SerializeField] private Material pavementMaterial;


    private GridTile[,] gridMatrix;

    [Header("City Zones")]
    [SerializeField] private List<CityZone> zones = new List<CityZone>();

    [HideInInspector]
    private Vector3 centerOfZones;

    public static CityManager Instance { get; private set; }
    public GridSpawner GridSpawner { get => gridSpawner; set => gridSpawner=value; }
    public int X { get => x; set => x=value; }
    public int Z { get => z; set => z=value; }
    public float GridSpacing { get => gridSpacing; set => gridSpacing=value; }
    public Vector3 GridOrigin { get => gridOrigin; set => gridOrigin=value; }
    public GameObject GridTilePrefab { get => gridTilePrefab; set => gridTilePrefab=value; }
    public Vector3 GridCenter { get => gridCenter; set => gridCenter=value; }
    public BuildingGenerator BuildingGenerator { get => buildingGenerator; set => buildingGenerator=value; }
    public RoadVisualizer Visualizer { get => visualizer; set => visualizer=value; }
    public Material PavementMaterial { get => pavementMaterial; set => pavementMaterial=value; }
    public GridTile[,] GridMatrix { get => gridMatrix; set => gridMatrix=value; }
    public List<CityZone> Zones { get => zones; set => zones=value; }
    public Vector3 CenterOfZones { get => centerOfZones; set => centerOfZones=value; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        //StartCoroutine(TestStart());
        GenerateGrid();

        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i].generateRoadsForZone)
            {
                visualizer.GenerateRoad(zones[i].zoneCenter, zones[i]);
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

    private IEnumerator TestStart()
    {
        GenerateGrid();
        for (int i = 0; i < zones.Count; i++)
        {
            if (zones[i].generateRoadsForZone)
            {
                visualizer.GenerateRoad(zones[i].zoneCenter, zones[i]);
                yield return new WaitForSeconds(1);
            }
        }

        if (zones.Count > 1)
        {
            visualizer.ConnectZones(zones);
        }

        visualizer.roadHelper.transform.position = new Vector3(visualizer.roadHelper.transform.position.x,
            gridCenter.y - 0.15f, visualizer.roadHelper.transform.position.z);

    }

    public void GenerateGrid()
    {
        gridCenter = new Vector3(x / 2, 0.45f, z / 2);
        gridMatrix = gridSpawner.GenerateGrid(x, z, gridSpacing, zones, gridOrigin, gridTilePrefab);
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

    [Header("Building Assets")]
    public int minUnits;
    public int maxUnits;
    public float startHeightOffset = 0.241f;
    public GameObject[] doorUnits;
    public GameObject[] baseUnits;
    public GameObject[] middleUnits;
    public GameObject[] topUnits;

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
    public Vector3 roadEndToCenter = new Vector3(100000000, 0, 10000000000);
    [HideInInspector]
    public Vector3 zoneCenter;
    
}
