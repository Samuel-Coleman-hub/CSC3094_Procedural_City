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

                float placementChanceMultipler = !gridMatrix[i, j].Zone.Equals(Zone.Agriculture) ? 3f : 0.25f;
                if (gridMatrix[i, j].TileType.Equals(TileType.Empty) && UnityEngine.Random.value * gridMatrix[i, j].CenterScore <= randomPercentage * placementChanceMultipler
                    && gridMatrix[i,j].NearRoad)
                {
                    

                    //Generate initial building
                    //GenerateBuilding(i, j, yScale, mat);
                    //Tile size

                    //Check neighbour cells
                    List<GridTile> emptyNeighbours;
                    int fullTiles = 0;
                    bool nextToRoad;

                    (emptyNeighbours, fullTiles, nextToRoad) = NumberOfEmptyNeighbours(i, j);

                    //JUST FOR DEBUGGING
                    //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //sphere.transform.position = centroid;

                    Material mat = materials[UnityEngine.Random.Range(0, materials.Count)];
                    //Randmoise scale
                    Vector3 centroid = FindCentroid(emptyNeighbours);
                    int gridLength = gridMatrix.GetLength(0) > gridMatrix.GetLength(1) ? gridMatrix.GetLength(0) : gridMatrix.GetLength(1);
                    float centerScorePercentage = (float)(gridMatrix[i, j].CenterScore / gridLength);

                    float yScaleMultiplier = gridMatrix[i, j].Zone.Equals(Zone.Industrial) ? 10f : 3f;
                    float yScale = Mathf.Clamp(heightCurve.Evaluate(centerScorePercentage) * UnityEngine.Random.value * yScaleMultiplier, 0.5f, 10);
                    float xzScale = Mathf.Clamp(heightCurve.Evaluate(centerScorePercentage) * UnityEngine.Random.value * 5f, 1f, emptyNeighbours.Count * 0.35f);
                    Debug.Log("xzScale " + xzScale);

                    StartCoroutine(GenerateBuilding(centroid.x, centroid.z, yScale, xzScale, mat, emptyNeighbours.Count));


                    //(int)Math.Min(emptyNeighbours.Count, Math.Max(1, heightCurve.Evaluate(centerScorePercentage) * UnityEngine.Random.value * 5f));
                    //for(int tileIndex = 0; tileIndex < buildingTileSize; tileIndex++)
                    //{
                    //    Debug.Log("Generating building " + emptyNeighbours[tileIndex].GetX() + " " + emptyNeighbours[tileIndex].GetY() + " for " + i + " " + j);
                    //    StartCoroutine(GenerateBuilding(emptyNeighbours[tileIndex].GetX(), emptyNeighbours[tileIndex].GetY(), yScale, mat));
                    //}
                }
            }
        }
    }

    private IEnumerator GenerateBuilding(float x, float z, float yScale, float xzScale, Material mat, int numTiles)
    {
        GameObject buildingObject = Instantiate(buildingPrefab, new Vector3(x, 1f,
            z), Quaternion.identity) as GameObject;
        //Debug.Log("In generateBuilding for " + x + " " + z);
        //GameObject tile = gridMatrix[x, z].Object;

        //Debug.Log("Before Object Instantiate " + x + " " + z);
        //GameObject buildingObject = Instantiate(buildingPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y + 1f,
        //    tile.transform.position.z), Quaternion.identity) as GameObject;
        //Debug.Log("after Object Instantiate " + x + " " + z);
        //buildingObject.name = "Building" + x + " " + z;
        //Debug.Log(buildingObject.name);

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

    private (List<GridTile>,int,bool) NumberOfEmptyNeighbours(int x, int z)
    {
        int fullTiles = 0;
        bool nextToRoad = false;

        int minX = Math.Max(x - 1, gridMatrix.GetLowerBound(0) + 1);
        int maxX = Math.Min(x + 1, gridMatrix.GetUpperBound(0) - 1);
        int minZ = Math.Max(z - 1, gridMatrix.GetLowerBound(1) + 1);
        int maxZ = Math.Min(z + 1, gridMatrix.GetUpperBound(1) - 1);

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
                    //Here need to work out if its next to another cell already in the emptyNeighbours list
                    //If it is then add to list if not then clear list
                    //Debug.Log("Square check incoming for neighbouring cell " + i + " " + j);
                    //if (emptyNeighbours.Count <= 1)
                    //{
                    //    Debug.Log("Auto add " + i + " " + j);
                    //    emptyNeighbours.Add(gridMatrix[i, j]);
                    //    gridMatrix[i, j].TileType = TileType.Building;
                    //}
                    //else
                    //{
                    //    emptyNeighbours.Add(gridMatrix[i, j]);

                    //    //This method works for now but should not be the final solution
                    //    gridMatrix[i, j].TileType = TileType.Building;
                    //    //Debug.Log("Should I look left?");
                    //    ////Left
                    //    //if (j - 1 >= 0 && j - 1 < gridMatrix.GetLength(1))
                    //    //{
                    //    //    Debug.Log(i + " " + j + " looking left");
                    //    //    if (emptyNeighbours.Contains(gridMatrix[i, j - 1]))
                    //    //    {
                    //    //        Debug.Log(i + " " + j + " left yes");
                    //    //        emptyNeighbours.Add(gridMatrix[i, j]);
                    //    //        //break;
                    //    //    }
                    //    //}

                    //    //Debug.Log("Should I look right?");
                    //    ////right
                    //    //if (j + 1 >= 0 && j + 1 < gridMatrix.GetLength(1))
                    //    //{
                    //    //    Debug.Log(i + " " + j + " looking right");
                    //    //    if (emptyNeighbours.Contains(gridMatrix[i, j + 1]))
                    //    //    {
                    //    //        Debug.Log(i + " " + j + " right yes");
                    //    //        emptyNeighbours.Add(gridMatrix[i, j]);
                    //    //       // break;
                    //    //    }
                    //    //}

                    //    //Debug.Log("Should I look up?");
                    //    ////up
                    //    //if (i - 1 >= 0 && i - 1 < gridMatrix.GetLength(0))
                    //    //{
                    //    //    Debug.Log(i + " " + j + " looking up");
                    //    //    if (emptyNeighbours.Contains(gridMatrix[i - 1, j]))
                    //    //    {
                    //    //        Debug.Log(i + " " + j + " up yes");
                    //    //        emptyNeighbours.Add(gridMatrix[i, j]);
                    //    //       // break;
                    //    //    }
                    //    //}

                    //    //Debug.Log("Should I look down?");
                    //    ////down
                    //    //if (i + 1 >= 0 && i + 1 < gridMatrix.GetLength(0))
                    //    //{
                    //    //    Debug.Log(i + " " + j + " looking down");
                    //    //    if (emptyNeighbours.Contains(gridMatrix[i + 1, j]))
                    //    //    {
                    //    //        Debug.Log(i + " " + j + " down yes");
                    //    //        emptyNeighbours.Add(gridMatrix[i, j]);
                    //    //       // break;
                    //    //    }
                    //    //}

                    //    //Debug.Log(i + " " + j + " is not in a square");
                    //    //emptyNeighbours.Clear();
                    //    //emptyNeighbours.Add(gridMatrix[x, z]);
                    //}
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

        return (emptyNeighbours, fullTiles, nextToRoad);
        ////I could make this a four loop
        //int emptyNeighbours = 4;

        ////Left
        //if (z-1 >= 0 && z-1 < gridMatrix.GetLength(1))
        //{
        //    if (!gridMatrix[x, z-1].TileType.Equals(TileType.Empty))
        //    {
        //        emptyNeighbours--;
        //    }
        //}

        ////right
        //if (z+1 >= 0 && z+1 < gridMatrix.GetLength(1))
        //{
        //    if (!gridMatrix[x, z+1].TileType.Equals(TileType.Empty))
        //    {
        //        emptyNeighbours--;
        //    }
        //}

        ////up
        //if (x-1 >= 0 && x-1 < gridMatrix.GetLength(0))
        //{
        //    if (!gridMatrix[x-1, z].TileType.Equals(TileType.Empty))
        //    {
        //        emptyNeighbours--;
        //    }
        //}

        ////down
        //if (x+1 >= 0 && x+1 < gridMatrix.GetLength(0))
        //{
        //    if (!gridMatrix[x+1, z].TileType.Equals(TileType.Empty))
        //    {
        //        emptyNeighbours--;
        //    }
        //}

        //bool allEmpty = emptyNeighbours <= 0;

        //return (allEmpty, emptyNeighbours);

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
