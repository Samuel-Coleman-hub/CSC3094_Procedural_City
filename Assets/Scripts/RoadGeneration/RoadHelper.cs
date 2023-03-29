using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class RoadHelper : MonoBehaviour
{
    [SerializeField] CityManager cityManager;
    public GameObject roadStraight;
    public GameObject roadCorner;
    public GameObject roadIntersection;
    public GameObject roadCrossroad;
    public GameObject roadEnd;

    //Vector3Int as want to compare positions, wont be accurate with floats
    Dictionary<Vector3Int, GameObject> roadDict = new Dictionary<Vector3Int, GameObject>();

    //Roads that might need fixing, hashset so no duplicates
    HashSet<Vector3Int> fixRoadPossibilities = new HashSet<Vector3Int>();

    public void PlaceRoad(Vector3 startPos, Vector3Int dir, int length)
    {
        Quaternion rotation = Quaternion.identity;
        if(dir.x == 0)
        {
            rotation = Quaternion.Euler(0, 90, 0);
        }

        for(int i = 0; i < length; i++)
        {
            
            //Next position of road
            Vector3Int pos = Vector3Int.RoundToInt(startPos + dir * i);
            //Checks if position is within the grid
            
            if(pos.x >= cityManager.x || pos.z >= cityManager.z || pos.x <= 0 || pos.z <= 0)
            {
                break;
            }

            //Checks that a road object has not alredy been placed at position
            if (roadDict.ContainsKey(pos))
            {
                continue;
            }
            //Instantiate road object
            GameObject road = Instantiate(roadStraight, pos, rotation, transform);
            roadDict.Add(pos, road);
            cityManager.gridMatrix[pos.x, pos.z].TileType = TileType.Road;


            //Mark positions on the opposite sides of the road as being next to the road
            if (dir.x != 0 && (pos.z - 1 >= 0 && pos.z + 1 < cityManager.x))
            {
                cityManager.gridMatrix[pos.x, pos.z + 1].NearRoad = true;
                cityManager.gridMatrix[pos.x, pos.z - 1].NearRoad = true;
            }
            else if (dir.z != 0 && (pos.x - 1 >= 0 && pos.x + 1 < cityManager.x))
            {
                cityManager.gridMatrix[pos.x + 1, pos.z].NearRoad = true;
                cityManager.gridMatrix[pos.x - 1, pos.z].NearRoad = true;
            }

            //Debug.Log("Position " + pos.x + " " + pos.z + " direction: " + dir);

            //Add positions to fix road possibilites, if we think they might need fixing
            if (i == 0 || i == length - 1)
            {
                fixRoadPossibilities.Add(pos);
            }
        }
    }

    public void FixRoad()
    {
        foreach(Vector3Int pos in fixRoadPossibilities)
        {
            Debug.Log(pos.x + " " + pos.z);
            //Get neighbour road directions
            List<Direction> neighbourDir = PlacementHelper.FindNeighbour(pos, roadDict.Keys);

            Quaternion rotation = Quaternion.identity;

            if(neighbourDir.Count == 1)
            {
                Destroy(roadDict[pos]);
                if (neighbourDir.Contains(Direction.Down))
                {
                    rotation = Quaternion.Euler(0, 90, 0);
                    if (RoadsNearbyInGrid(pos.x, pos.z)) { cityManager.gridMatrix[pos.x, pos.z + 1].NearRoad = true; }
                    //Debug.Log("road end down " + pos.x + " " + pos.z);
                }
                else if (neighbourDir.Contains(Direction.Left))
                {
                    rotation = Quaternion.Euler(0, 180, 0);
                    if (RoadsNearbyInGrid(pos.x, pos.z)) { cityManager.gridMatrix[pos.x-1, pos.z].NearRoad = true; }
                    if (RoadsNearbyInGrid(pos.x, pos.z)) { cityManager.gridMatrix[pos.x+1, pos.z].NearRoad = true; }
                    //Debug.Log("road end left " + pos.x + " " + pos.z);
                    
                }
                else if (neighbourDir.Contains(Direction.Up))
                {
                    rotation = Quaternion.Euler(0, -90, 0);
                    if (RoadsNearbyInGrid(pos.x, pos.z)) { cityManager.gridMatrix[pos.x, pos.z -1].NearRoad = true; }
                    //Debug.Log("road end up " + pos.x + " " + pos.z);
                }
                roadDict[pos] = Instantiate(roadEnd, pos, rotation, transform);

            }
            else if(neighbourDir.Count == 2)
            {
                if(neighbourDir.Contains(Direction.Up) && neighbourDir.Contains(Direction.Down)
                    || neighbourDir.Contains(Direction.Right) && neighbourDir.Contains(Direction.Left))
                {
                    continue;
                }

                //Checking for where a bend is needed
                Destroy(roadDict[pos]);
                if (neighbourDir.Contains(Direction.Down) && neighbourDir.Contains(Direction.Right))
                {
                    rotation = Quaternion.Euler(0, 90, 0);
                    if (RoadsNearbyInGrid(pos.x,pos.z)) { cityManager.gridMatrix[pos.x - 1, pos.z].NearRoad = true; }
                    //Debug.Log("Bend down and right " + pos.x + " " + pos.z);
                }
                else if (neighbourDir.Contains(Direction.Left) && neighbourDir.Contains(Direction.Down))
                {
                    rotation = Quaternion.Euler(0, 180, 0);
                    if (RoadsNearbyInGrid(pos.x, pos.z)) { cityManager.gridMatrix[pos.x + 1, pos.z].NearRoad = true; }
                    //Debug.Log("Bend Left and Down " + pos.x + " " + pos.z);
                }
                else if (neighbourDir.Contains(Direction.Up) && neighbourDir.Contains(Direction.Left))
                {
                    rotation = Quaternion.Euler(0, -90, 0);
                    if (RoadsNearbyInGrid(pos.x, pos.z)) { cityManager.gridMatrix[pos.x + 1, pos.z].NearRoad = true; }
                    //Debug.Log("Bend Up and Left " + pos.x + " " + pos.z);
                }
                roadDict[pos] = Instantiate(roadCorner, pos, rotation, transform);

            }
            else if(neighbourDir.Count == 3)
            {
                //Check for intersection
                Destroy(roadDict[pos]);
                if (neighbourDir.Contains(Direction.Up) && neighbourDir.Contains(Direction.Right)
                    && neighbourDir.Contains(Direction.Down))
                {
                    rotation = Quaternion.Euler(0, 90, 0);
                }
                else if (neighbourDir.Contains(Direction.Left) && neighbourDir.Contains(Direction.Right)
                    && neighbourDir.Contains(Direction.Down))
                {
                    rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (neighbourDir.Contains(Direction.Up) && neighbourDir.Contains(Direction.Left)
                    && neighbourDir.Contains(Direction.Down))
                {
                    rotation = Quaternion.Euler(0, -90, 0);
                }
                roadDict[pos] = Instantiate(roadIntersection, pos, rotation, transform);
            }
            else
            {
                Destroy(roadDict[pos]);
                roadDict[pos] = Instantiate(roadCrossroad, pos, rotation, transform);
            }
        }
    }

    private bool RoadsNearbyInGrid(int x, int z)
    {
        if((x - 1 >= 0 && x + 1 < cityManager.x) && (z - 1 >= 0 && z + 1 < cityManager.x))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
