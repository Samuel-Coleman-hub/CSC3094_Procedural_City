using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProducer : MonoBehaviour
{

    private Quaternion buildingRotation = Quaternion.identity;

    public void Build(List<GridTile> tiles, NearRoadDirection roadDirection, int buildingWidth, int buildingHeight)
    {
        int minUnits = tiles[0].Zone.minUnits;
        int maxUnits = tiles[0].Zone.maxUnits;
        float startHeightOffset = tiles[0].Zone.startHeightOffset;
        GameObject[] doorUnits = tiles[0].Zone.doorUnits;
        GameObject[] baseUnits = tiles[0].Zone.baseUnits;
        GameObject[] middleUnits = tiles[0].Zone.middleUnits;
        GameObject[] topUnits = tiles[0].Zone.topUnits;

        bool[] storyInTile = new bool[6];

        GameObject buildingEmpty = new GameObject();
        buildingEmpty.transform.parent = this.transform;
        buildingEmpty.name = "Building";

        Material buildingMat = tiles[0].Zone.buildingMainColourMaterials[Random.Range(0, 
            tiles[0].Zone.buildingMainColourMaterials.Count)];

        switch (roadDirection)
        {
            case NearRoadDirection.North:
                buildingRotation = Quaternion.Euler(0, 0, 0);
                break;
            case NearRoadDirection.East:
                buildingRotation = Quaternion.Euler(0, -90, 0);
                break;
            case NearRoadDirection.South:
                buildingRotation = Quaternion.Euler(0, 180, 0);
                break;
            case NearRoadDirection.West:
                buildingRotation = Quaternion.Euler(0, 90, 0);
                break;
        }

        buildingEmpty.name = "Building " + roadDirection;

        for (int i = 0; i < buildingWidth; i++)
        {
            //They are spawning on top of each other
            tiles[i].ChildObject = buildingEmpty;
            Vector3 tilePos = new Vector3(tiles[i].GetX(), 0f, tiles[i].GetZ());

            GameObject storyEmpty = new GameObject();
            storyEmpty.name = "Story " + tiles[i].GetX() + ", " + tiles[i].GetZ();
            storyEmpty.transform.parent = buildingEmpty.transform;

            //Randomly decided how many modular units we are going to use
            int targetUnits = Random.Range(minUnits, maxUnits);

            //Defines height to ensure we place object above the previous one
            //Change this to be variable start height
            float heighOffset = startHeightOffset;
            heighOffset += SpawnPieceLayer(baseUnits, heighOffset, tilePos, storyEmpty.transform, buildingMat);

            //Loop through and spawn middle part pieces
            for (int j = 0; j < buildingHeight; j++)
            {
                heighOffset += SpawnPieceLayer(middleUnits, heighOffset, tilePos, storyEmpty.transform, buildingMat);
            }

            SpawnPieceLayer(topUnits, heighOffset, tilePos ,storyEmpty.transform);
        }
        
    }

    //Gets the height that our last unit was at 
    private float SpawnPieceLayer(GameObject[] unitsArray, float inputHeight, Vector3 pos, Transform storyEmpty, Material buildingMat = default)
    {
        //Picks a random object from the appropriate unit array to place next
        Transform randomTransform = unitsArray[Random.Range(0, unitsArray.Length)].transform;
        GameObject clone = Instantiate(randomTransform.gameObject, new Vector3(pos.x, inputHeight, pos.z), buildingRotation) as GameObject;
        //Mesh cloneMesh = clone.GetComponentInChildren<MeshFilter>().mesh;
        //Bounds baseBounds = cloneMesh.bounds;
        Collider collider = clone.GetComponent<Collider>();
        float heightOffset = collider.bounds.size.y;

        clone.transform.SetParent(storyEmpty.transform);

        if(buildingMat != default)
        {
            Renderer cloneRenderer = clone.GetComponent<Renderer>();
            Material[] materials = cloneRenderer.materials;
            materials[0] = buildingMat;
            cloneRenderer.materials = materials;
        }
        

        return heightOffset;
    }

    
}
