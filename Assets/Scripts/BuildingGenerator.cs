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


                //Check neighbour cells
                bool allEmpty = false;
                int numOfEmptyCells = 0;

                (allEmpty, numOfEmptyCells) = NumberOfEmptyNeighbours(i,j);

                if (gridMatrix[i, j].TileType.Equals(TileType.Empty) && UnityEngine.Random.value * gridMatrix[i, j].CenterScore <= randomPercentage * 2 && numOfEmptyCells <= 3
                    )
                {

                    GameObject tile = gridMatrix[i, j].Object;
                    GameObject buildingObject = Instantiate(buildingPrefab, new Vector3(tile.transform.position.x, tile.transform.position.y + 1f,
                        tile.transform.position.z), Quaternion.identity);
                    buildingObject.GetComponent<MeshRenderer>().material = materials[UnityEngine.Random.Range(0, materials.Count)];

                    //Randomise scale of building
                    int gridLength = gridMatrix.GetLength(0) > gridMatrix.GetLength(1) ? gridMatrix.GetLength(0) : gridMatrix.GetLength(1);
                    float centerScorePercentage = (float)(gridMatrix[i, j].CenterScore / gridLength);
                    float yScale = Mathf.Clamp(heightCurve.Evaluate(centerScorePercentage) * UnityEngine.Random.value * 5f ,0.5f,10);

                    buildingObject.transform.position += Vector3.up * yScale / 2;
                    buildingObject.transform.localScale += Vector3.up * yScale;

                    gridMatrix[i, j].ChildObject = buildingObject;
                    gridMatrix[i, j].TileType = TileType.Building;
                }
            }
        }
    }

    private (bool,int) NumberOfEmptyNeighbours(int x, int z)
    {
        //I could make this a four loop
        int emptyNeighbours = 4;
        bool allEmpty = false;

        //Left
        if (z-1 >= 0 && z-1 < gridMatrix.GetLength(1))
        {
            if (!gridMatrix[x, z-1].TileType.Equals(TileType.Empty))
            {
                emptyNeighbours--;
                allEmpty = false;
            }
        }



        //right
        if (z+1 >= 0 && z+1 < gridMatrix.GetLength(1))
        {
            if (!gridMatrix[x, z+1].TileType.Equals(TileType.Empty))
            {
                emptyNeighbours--;
                allEmpty = false;
            }
        }

        //up
        if (x-1 >= 0 && x-1 < gridMatrix.GetLength(0))
        {
            if (!gridMatrix[x-1, z].TileType.Equals(TileType.Empty))
            {
                emptyNeighbours--;
                allEmpty = false;
            }
        }

        //down
        if (x+1 >= 0 && x+1 < gridMatrix.GetLength(0))
        {
            if (!gridMatrix[x+1, z].TileType.Equals(TileType.Empty))
            {
                emptyNeighbours--;
                allEmpty = false;
            }
        }

        allEmpty = true;

        return (allEmpty, emptyNeighbours);

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
