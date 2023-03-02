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

                if (gridMatrix[i, j].TileType.Equals(TileType.Empty) && UnityEngine.Random.value * gridMatrix[i, j].CenterScore <= randomPercentage * 2)
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

                    //buildingObject.transform.localScale = new Vector3(buildingObject.transform.localScale.x,
                    //    yScale, buildingObject.transform.localScale.z);
                    //buildingObject.transform.position += new Vector3(0f, yScale * 0.1f, 0f);

                    gridMatrix[i, j].ChildObject = buildingObject;
                    gridMatrix[i, j].TileType = TileType.Building;
                }
            }
        }
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
