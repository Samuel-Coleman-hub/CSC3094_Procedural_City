using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BuildingProducer : MonoBehaviour
{
    public int minUnits;
    public int maxUnits;
    public GameObject[] baseUnits;
    public GameObject[] middleUnits;
    public GameObject[] topUnits;

    public void Build(List<GridTile> tiles)
    {
        GameObject buildingEmpty = new GameObject();
        buildingEmpty.name = "Building";

        foreach (GridTile tile in tiles)
        {
            Vector3 tilePos = new Vector3(tile.GetX(), 0, tile.GetY());

            GameObject storyEmpty = new GameObject();
            storyEmpty.name = "Story " + tile.GetX() + ", " + tile.GetY();
            storyEmpty.transform.parent = buildingEmpty.transform;

            //Randomly decided how many modular units we are going to use
            int targetUnits = Random.Range(minUnits, maxUnits);

            //Defines height to ensure we place object above the previous one
            float heighOffset = 0;
            heighOffset += SpawnPieceLayer(baseUnits, heighOffset, tilePos, storyEmpty.transform);

            //Loop through and spawn middle part pieces
            for (int i = 0; i < targetUnits; i++)
            {
                heighOffset += SpawnPieceLayer(middleUnits, heighOffset, tilePos, storyEmpty.transform);
            }

            SpawnPieceLayer(topUnits, heighOffset, tilePos ,storyEmpty.transform);
        }
        
    }

    //Gets the height that our last unit was at 
    private float SpawnPieceLayer(GameObject[] unitsArray, float inputHeight, Vector3 pos, Transform storyEmpty)
    {
        //Picks a random object from the appropriate unit array to place next
        Transform randomTransform = unitsArray[Random.Range(0, unitsArray.Length)].transform;
        GameObject clone = Instantiate(randomTransform.gameObject, new Vector3(pos.x, inputHeight, pos.z), transform.rotation) as GameObject;
        GameObject clone1 = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), new Vector3(pos.x, inputHeight, pos.z), transform.rotation) as GameObject;
        Mesh cloneMesh = clone.GetComponentInChildren<MeshFilter>().mesh;
        Bounds baseBounds = cloneMesh.bounds;
        float heightOffset = baseBounds.size.y;

        clone.transform.SetParent(storyEmpty.transform);

        return heightOffset;
    }
}
