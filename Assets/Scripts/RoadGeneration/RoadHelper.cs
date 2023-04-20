using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    //DEBUGGING
    int counter;
    bool mainRoad = false;

    public void PlaceRoad(Vector3 startPos, Vector3Int dir, int length, CityZone zone = default)
    {
        if(zone == default)
        {
            mainRoad = true;
        }
        
        Quaternion rotation = Quaternion.identity;
        if(dir.x == 0)
        {
            rotation = Quaternion.Euler(0, 90, 0);
        }

        for(int i = 0; i < length; i++)
        {
            //Next position of road
            Vector3Int pos = Vector3Int.RoundToInt(startPos + dir * i);


            //YOU CAN REMOVE GRID CHECK ZONE IF IT HAS TO BE IN ZONE ANYWAY
            //Checks if position is within the grid and in the zone
            if(pos.x >= cityManager.x || pos.z >= cityManager.z || pos.x <= 0 || pos.z <= 0 ||
                (cityManager.gridMatrix[pos.x,pos.z].Zone != zone && !mainRoad))
            {
                fixRoadPossibilities.Add(roadDict.Last().Key);
                break;
            }

            //Checks that a road object has not alredy been placed at position
            if (roadDict.ContainsKey(pos))
            {
                fixRoadPossibilities.Add(pos);
                continue;
            }

            if (cityManager.gridMatrix[pos.x, pos.z].TileType.Equals(TileType.Empty))
            {
                GameObject road = Instantiate(roadStraight, pos, rotation, transform);
                roadDict.Add(pos, road);
                cityManager.gridMatrix[pos.x, pos.z].TileType = TileType.Road;
            }
            else
            {
                fixRoadPossibilities.Add(pos);
            }
            //Instantiate road object

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

    public Vector3 FixRoad(CityZone zone, List<Vector3> posToFix = default)
    {
        counter = 0;
        if(posToFix != null)
        {
            foreach (Vector3 pos in posToFix)
            {
                fixRoadPossibilities.Add(Vector3Int.RoundToInt(pos));
            }
        }
        
        Vector3 connectorRoadEnd = new Vector3();
        foreach(Vector3Int pos in fixRoadPossibilities)
        {
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
                counter++; 
                //Checks if this road end is closer to the center than the previous, if so add to list
                if(((Vector3.Distance(connectorRoadEnd, cityManager.centerOfZones) > Vector3.Distance(pos, cityManager.centerOfZones)) || connectorRoadEnd == null)
                    && cityManager.gridMatrix[pos.x,pos.z].Zone.Equals(zone))
                {
                    connectorRoadEnd = pos;
                    GameObject temp = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Capsule));
                    temp.transform.position = pos;

                }
                else
                {
                    GameObject temp = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere));
                    temp.transform.position = pos;
                    temp.name = "Sphere " + counter;
                }

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
        return connectorRoadEnd;
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
