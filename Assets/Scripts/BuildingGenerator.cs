using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    //Building generation variables
    private List<Material> materials = new List<Material>();
    private GameObject buildingPrefab = null;
    private float randomPercentage;
    private GridTile[,] gridMatrix;
    private BuildingProducer producer;

    private void Start()
    {
        producer = GetComponent<BuildingProducer>();
    }

    private void SpawnBuildings()
    {
        for (int i = 0; i < gridMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < gridMatrix.GetLength(1); j++)
            {
                if (gridMatrix[i, j].ChildObject != null)
                {
                    Destroy(gridMatrix[i, j].ChildObject);
                    gridMatrix[i, j].TileType = TileType.Empty;
                }

                CityZone zone = gridMatrix[i, j].Zone;
                float distToCenter = Vector3.Distance(zone.zoneCenter, new Vector3(i, 0, j));

                float placementChanceMultipler = zone.chanceOfBuildingPlacement;
                if (gridMatrix[i, j].TileType.Equals(TileType.Empty) && UnityEngine.Random.value * distToCenter <= randomPercentage * placementChanceMultipler
                    && !gridMatrix[i,j].NearRoadDirection.Equals(NearRoadDirection.None))
                {
                    List<GridTile> emptyNeighbours;
                    emptyNeighbours = NumberOfEmptyNeighbours(i, j);


                    //Material mat = materials[UnityEngine.Random.Range(0, materials.Count)];
                    //Vector3 centroid = FindCentroid(emptyNeighbours);


                    //int gridLength = gridMatrix.GetLength(0) > gridMatrix.GetLength(1) ? gridMatrix.GetLength(0) : gridMatrix.GetLength(1);
                    //float centerScorePercentage = (float)(gridMatrix[i, j].CenterScore / gridLength);
                    
                    double centerScore = Math.Floor(Math.Sqrt(Math.Pow(((gridMatrix[i, j].Zone.zoneCenter.x)-i), 2) 
                        + Math.Pow((gridMatrix[i, j].Zone.zoneCenter.z-j), 2)));
                    float centerScorePercentage = (float)(distToCenter / Math.Sqrt(gridMatrix[i,j].Zone.positionsInZone.Count));
                    centerScorePercentage *= 2;



                    float heightScaleMultiplier = zone.buildingHeightScaleMultiplier;
                    int buildingHeight = (int)Mathf.Clamp(zone.heightCenterCurve.Evaluate(centerScorePercentage) * UnityEngine.Random.value *
                        heightScaleMultiplier, zone.minBuildingHeight, zone.maxBuildingHeight);

                    float widthScaleMultiplier = zone.buildingWidthScaleMultiplier;
                    int buildingWidth = (int)Mathf.Clamp(zone.widthCenterCurve.Evaluate(centerScorePercentage) * UnityEngine.Random.value *
                        widthScaleMultiplier, Mathf.Clamp(zone.minBuildingWidth, 0f, emptyNeighbours.Count), Mathf.Clamp(zone.maxBuildingWidth, 1f, emptyNeighbours.Count));

                    Debug.Log("height " + i + ", " + j + ": " + buildingHeight);
                    Debug.Log("width " + i + ", " + j + ": " + buildingWidth);
                    Debug.Log("center distance " + i + ", " + j + ": " + distToCenter);
                    Debug.Log("center score " + i + ", " + j + ": " + centerScore);
                    Debug.Log("center percentage " + i + ", " + j + ": " + centerScorePercentage + " where tiles in zone are " + zone.positionsInZone.Count);
                    Debug.Log("Evaluating center percentage on curve " + gridMatrix[i, j].Zone.heightCenterCurve.Evaluate(centerScorePercentage));


                    producer.Build(emptyNeighbours, gridMatrix[i,j].NearRoadDirection, buildingWidth, buildingHeight);

                }
            }
        }
    }

    private IEnumerator GenerateBuilding(float x, float z, float yScale, float xzScale, Material mat, int numTiles)
    {
        GameObject buildingObject = Instantiate(buildingPrefab, new Vector3(x, 1f,
            z), Quaternion.identity) as GameObject;

        yield return new WaitForSeconds(0f);

        buildingObject.GetComponent<MeshRenderer>().material = mat;

        ////Randomise scale of building
        buildingObject.transform.position += Vector3.up * yScale / 2;
        buildingObject.transform.localScale += Vector3.up * yScale;
        Vector3 scale = buildingObject.transform.localScale;
        //buildingObject.transform.localScale = new Vector3(scale.x * numTiles * 0.45f, scale.y, scale.z * numTiles * 0.45f);
        buildingObject.transform.localScale = new Vector3(xzScale, scale.y, xzScale);
        //buildingObject.transform.parent = gridMatrix[x, z].Object.transform;

        //gridMatrix[x, z].ChildObject = buildingObject;
        //gridMatrix[x, z].TileType = TileType.Building;
    }

    private List<GridTile> NumberOfEmptyNeighbours(int x, int z)
    {
        int minX = Math.Max(x - 1, gridMatrix.GetLowerBound(0) + 1);
        int maxX = Math.Min(x + 1, gridMatrix.GetUpperBound(0) - 1);
        int minZ = Math.Max(z - 1, gridMatrix.GetLowerBound(1) + 1);
        int maxZ = Math.Min(z + 1, gridMatrix.GetUpperBound(1) - 1);

        List<GridTile> neighbourRoadTiles = new List<GridTile>();
        List<GridTile> emptyNeighbours = new List<GridTile>();
        emptyNeighbours.Add(gridMatrix[x, z]);
        List<int> numBuildingsPerRow = new List<int>();


        for(int i = minX; i <= maxX; i++)
        {
            int zCount = 0;
            for (int j = minZ; j <= maxZ; j++)
            {
               
                if (gridMatrix[i, j].TileType.Equals(TileType.Empty))
                {
                    emptyNeighbours.Add(gridMatrix[i, j]);
                    gridMatrix[i, j].TileType = TileType.Building;
                    zCount++;
                }
            }
            numBuildingsPerRow.Add(zCount);
        }

        emptyNeighbours = emptyNeighbours.Distinct().ToList();

        if(emptyNeighbours.Count == 3 || emptyNeighbours.Count == 5 || emptyNeighbours.Count == 7)
        {
            int unique = numBuildingsPerRow.Distinct().First();
            if (unique == 1)
            {
                Debug.Log("It is one in column " + numBuildingsPerRow.IndexOf(unique));
            }
        }

        return emptyNeighbours;
    }

    public Vector3 FindCentroid(List<GridTile> neighbouringTiles)
    {
        Vector3 centroid = new Vector3(0,0,0);
        int numPoints = neighbouringTiles.Count;

        foreach(GridTile tile in neighbouringTiles) 
        {
            centroid += tile.Object.transform.position;
        }

        centroid /= numPoints;

        return centroid;
    }

    public void GenerateBuildings(List<Material> materials, GameObject buildingPrefab, float randomPercentage, GridTile[,] gridMatrix)
    {
        this.materials = materials;
        this.buildingPrefab = buildingPrefab;
        this.randomPercentage = randomPercentage;
        this.gridMatrix = gridMatrix;
        SpawnBuildings();
    }
}
