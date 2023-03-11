using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
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

                if (gridMatrix[i, j].TileType.Equals(TileType.Empty) && UnityEngine.Random.value * gridMatrix[i, j].CenterScore <= randomPercentage * 3f && gridMatrix[i,j].NearRoad)
                {
                    Material mat = materials[UnityEngine.Random.Range(0, materials.Count)];
                    //Randmoise scale

                    int gridLength = gridMatrix.GetLength(0) > gridMatrix.GetLength(1) ? gridMatrix.GetLength(0) : gridMatrix.GetLength(1);
                    float centerScorePercentage = (float)(gridMatrix[i, j].CenterScore / gridLength);
                    float yScale = Mathf.Clamp(heightCurve.Evaluate(centerScorePercentage) * UnityEngine.Random.value * 5f, 0.5f, 10);

                    //Generate initial building
                    //GenerateBuilding(i, j, yScale, mat);
                    //Tile size

                    //Check neighbour cells
                    List<GridTile> emptyNeighbours;
                    int fullTiles = 0;
                    bool nextToRoad;

                    (emptyNeighbours, fullTiles, nextToRoad) = NumberOfEmptyNeighbours(i, j);

                    int buildingTileSize = emptyNeighbours.Count;//(int)Math.Min(emptyNeighbours.Count, Math.Max(1, heightCurve.Evaluate(centerScorePercentage) * UnityEngine.Random.value * 5f));
                    for(int tileIndex = 0; tileIndex < buildingTileSize; tileIndex++)
                    {
                        GenerateBuilding(emptyNeighbours[tileIndex].GetX(), emptyNeighbours[tileIndex].GetY(), yScale, mat);
                    }
                }
            }
        }
    }

    private void GenerateBuilding(int i, int j, float yScale, Material mat)
    {
        GameObject tile = gridMatrix[i, j].Object;

        GameObject buildingObject = Instantiate(buildingPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y + 1f,
            tile.transform.position.z), Quaternion.identity);
        buildingObject.GetComponent<MeshRenderer>().material = mat;

        //Randomise scale of building
        buildingObject.transform.position += Vector3.up * yScale / 2;
        buildingObject.transform.localScale += Vector3.up * yScale;

        gridMatrix[i, j].ChildObject = buildingObject;
        gridMatrix[i, j].TileType = TileType.Building;
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
        //THERE IS AN ERROR HERE IT IS NOT SEARCHING THROUGH ALL THE NEIGHBOURS
        for(int i = minX; i <= maxX; i++)
        {
            for (int j = minZ; j <= maxZ; j++)
            {
                Debug.Log("IS A Neighbour of " + x + " " + z + " is: " + i + " " + j);
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(i, 0, j);
                if (gridMatrix[i, j].TileType.Equals(TileType.Empty))
                {
                    //Here need to work out if its next to another cell already in the emptyNeighbours list
                    //If it is then add to list if not then clear list
                    Debug.Log("Square check incoming for neighbouring cell " + i + " " + j);
                    if (emptyNeighbours.Count <= 1)
                    {
                        Debug.Log("Auto add " + i + " " + j);
                        emptyNeighbours.Add(gridMatrix[i, j]);
                    }
                    else
                    {
                        Debug.Log("Should I look left?");
                        //Left
                        if (j - 1 >= 0 && j - 1 < gridMatrix.GetLength(1))
                        {
                            Debug.Log(i + " " + j + " looking left");
                            if (emptyNeighbours.Contains(gridMatrix[i, j - 1]))
                            {
                                Debug.Log(i + " " + j + " left yes");
                                emptyNeighbours.Add(gridMatrix[i, j]);
                                break;
                            }
                        }

                        Debug.Log("Should I look right?");
                        //right
                        if (j + 1 >= 0 && j + 1 < gridMatrix.GetLength(1))
                        {
                            Debug.Log(i + " " + j + " looking right");
                            if (emptyNeighbours.Contains(gridMatrix[i, j + 1]))
                            {
                                Debug.Log(i + " " + j + " right yes");
                                emptyNeighbours.Add(gridMatrix[i, j]);
                                break;
                            }
                        }

                        Debug.Log("Should I look up?");
                        //up
                        if (i - 1 >= 0 && i - 1 < gridMatrix.GetLength(0))
                        {
                            Debug.Log(i + " " + j + " looking up");
                            if (emptyNeighbours.Contains(gridMatrix[i - 1, j]))
                            {
                                Debug.Log(i + " " + j + " up yes");
                                emptyNeighbours.Add(gridMatrix[i, j]);
                                break;
                            }
                        }

                        Debug.Log("Should I look down?");
                        //down
                        if (i + 1 >= 0 && i + 1 < gridMatrix.GetLength(0))
                        {
                            Debug.Log(i + " " + j + " looking down");
                            if (emptyNeighbours.Contains(gridMatrix[i + 1, j]))
                            {
                                Debug.Log(i + " " + j + " down yes");
                                emptyNeighbours.Add(gridMatrix[i, j]);
                                break;
                            }
                        }

                        Debug.Log(i + " " + j + " is not in a square");
                        //emptyNeighbours.Clear();
                        //emptyNeighbours.Add(gridMatrix[x, z]);
                    }
                    Debug.Log("End of search for " + i + " " + j);

                }

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
        yield return new WaitForSeconds(1);
    }
}
