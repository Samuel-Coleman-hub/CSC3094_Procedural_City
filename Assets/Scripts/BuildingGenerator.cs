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
    [SerializeField] private AnimationCurve heightCurve;
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

                float placementChanceMultipler = gridMatrix[i, j].Zone.chanceOfBuildingPlacement;
                if (gridMatrix[i, j].TileType.Equals(TileType.Empty) && UnityEngine.Random.value * gridMatrix[i, j].CenterScore <= randomPercentage * placementChanceMultipler
                    && gridMatrix[i,j].NearRoad)
                {

                    List<GridTile> emptyNeighbours;
                    emptyNeighbours = NumberOfEmptyNeighbours(i, j);

                    Material mat = materials[UnityEngine.Random.Range(0, materials.Count)];

                    Vector3 centroid = FindCentroid(emptyNeighbours);
                    int gridLength = gridMatrix.GetLength(0) > gridMatrix.GetLength(1) ? gridMatrix.GetLength(0) : gridMatrix.GetLength(1);
                    float centerScorePercentage = (float)(gridMatrix[i, j].CenterScore / gridLength);

                    float yScaleMultiplier = gridMatrix[i, j].Zone.buildingYScaleMultiplier;
                    float yScale = Mathf.Clamp(heightCurve.Evaluate(centerScorePercentage) * UnityEngine.Random.value * yScaleMultiplier, 0.5f, 10);
                    float xzScale = Mathf.Clamp(heightCurve.Evaluate(centerScorePercentage) * UnityEngine.Random.value * 5f, 1f, emptyNeighbours.Count * 0.35f);
                    Debug.Log("xzScale " + xzScale);

                    producer.Build(emptyNeighbours);

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
        int fullTiles = 0;

        int minX = Math.Max(x - 1, gridMatrix.GetLowerBound(0) + 1);
        int maxX = Math.Min(x + 1, gridMatrix.GetUpperBound(0) - 1);
        int minZ = Math.Max(z - 1, gridMatrix.GetLowerBound(1) + 1);
        int maxZ = Math.Min(z + 1, gridMatrix.GetUpperBound(1) - 1);

        List<GridTile> neighbourRoadTiles = new List<GridTile>();
        List<GridTile> emptyNeighbours = new List<GridTile>();
        emptyNeighbours.Add(gridMatrix[x, z]);
        //GridTile[,] emptyTiles = new GridTile[1, 2];
        //emptyTiles[1, 1] = gridMatrix[x, z];
        Debug.Log("This is for x: " + x + " z: " + z);
        List<int> numBuildingsPerRow = new List<int>();


        for(int i = minX; i <= maxX; i++)
        {
            int zCount = 0;
            for (int j = minZ; j <= maxZ; j++)
            {
                Debug.Log("IS A Neighbour of " + x + " " + z + " is: " + i + " " + j);
                //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube.transform.position = new Vector3(i, 0, j);
                if (gridMatrix[i, j].TileType.Equals(TileType.Empty))
                {
                    emptyNeighbours.Add(gridMatrix[i, j]);
                    gridMatrix[i, j].TileType = TileType.Building;
                    zCount++;
                    
                    Debug.Log("End of search for " + i + " " + j);
                }
            }
            numBuildingsPerRow.Add(zCount);
        }

        Debug.Log("For " + x + " " + z + " "+ emptyNeighbours.Count + "  and distinct" + emptyNeighbours.Distinct().ToList().Count);
        emptyNeighbours = emptyNeighbours.Distinct().ToList();

        if(emptyNeighbours.Count == 3 || emptyNeighbours.Count == 5 || emptyNeighbours.Count == 7)
        {
            Debug.Log("There is a problem at " + x + " " + z + " because count is " + emptyNeighbours.Count);
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

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0);
    }
}
