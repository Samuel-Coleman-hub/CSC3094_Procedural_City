using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class RoadHelper : MonoBehaviour
{
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

    private GridTile[,] gridMatrix;

    public void PlaceRoad(Vector3 startPos, Vector3Int dir, int length, CityZone zone = default)
    {
        gridMatrix = CityManager.Instance.GridMatrix;
        Vector3 connectorRoadEnd = new Vector3();

        if (zone == default)
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
            if ((pos.x >= CityManager.Instance.X || pos.z >= CityManager.Instance.Z || pos.x <= 0 || pos.z <= 0 ||
                (gridMatrix[pos.x, pos.z].Zone != zone && !mainRoad)) && roadDict.Count != 0 )
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

            Debug.Log("Pos on line 67 " + pos.x + " " + pos.z);
            if (gridMatrix[pos.x, pos.z].TileType.Equals(TileType.Empty) || gridMatrix[pos.x, pos.z].TileType.Equals(TileType.Pavement))
            {

                if (((Vector3.Distance(connectorRoadEnd, CityManager.Instance.CenterOfZones) > Vector3.Distance(pos, CityManager.Instance.CenterOfZones)) || connectorRoadEnd == null)
                    && gridMatrix[pos.x, pos.z].Zone.Equals(zone))
                {
                    connectorRoadEnd = pos;
                    zone.roadEndToCenter = connectorRoadEnd;
                    //GameObject temp = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Capsule));
                    //temp.transform.position = pos;
                }

                GameObject road = Instantiate(roadStraight, pos, rotation, transform);
                roadDict.Add(pos, road);
                gridMatrix[pos.x, pos.z].TileType = TileType.Road;
            }
            else
            {
                fixRoadPossibilities.Add(pos);
            }
            //Instantiate road object

            //Mark positions on the opposite sides of the road as being next to the road
            if (dir.x != 0 && (pos.z - 1 >= 0 && pos.z + 1 < CityManager.Instance.X))
            {
                //gridMatrix[pos.x, pos.z + 1].NearRoadDirection = NearRoadDirection.South;
                //gridMatrix[pos.x, pos.z - 1].NearRoadDirection = NearRoadDirection.North;

                CreatePavement(pos.x, pos.z +1, false, true, NearRoadDirection.South);
                CreatePavement(pos.x, pos.z -1, false, false, NearRoadDirection.North);

            }
            else if (dir.z != 0 && (pos.x - 1 >= 0 && pos.x + 1 < CityManager.Instance.X))
            {
                
                //gridMatrix[pos.x + 1, pos.z].NearRoadDirection = NearRoadDirection.East;
                //gridMatrix[pos.x - 1, pos.z].NearRoadDirection = NearRoadDirection.West;

                CreatePavement(pos.x + 1, pos.z, true, true, NearRoadDirection.East);
                CreatePavement(pos.x - 1, pos.z, true, false, NearRoadDirection.West);
            }

            //Debug.Log("Position " + pos.x + " " + pos.z + " direction: " + dir);

            //Add positions to fix road possibilites, if we think they might need fixing
            if (i == 0 || i == length - 1)
            {
                fixRoadPossibilities.Add(pos);
            }
        }
    }

    public void FixRoad(CityZone zone, List<Vector3> posToFix = default)
    {
        gridMatrix = CityManager.Instance.GridMatrix;

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
                    if (RoadsNearbyInGrid(pos.x, pos.z)) 
                    { 
                        //gridMatrix[pos.x, pos.z + 1].NearRoadDirection = NearRoadDirection.South;
                        CreatePavement(pos.x, pos.z +1, false, true, NearRoadDirection.South);
                    }
                    //Debug.Log("road end down " + pos.x + " " + pos.z);
                }
                else if (neighbourDir.Contains(Direction.Left))
                {
                    rotation = Quaternion.Euler(0, 180, 0);
                    if (RoadsNearbyInGrid(pos.x, pos.z)) 
                    { 
                        //gridMatrix[pos.x-1, pos.z].NearRoadDirection = NearRoadDirection.East;
                        CreatePavement(pos.x - 1, pos.z, true, false, NearRoadDirection.East);
                    }
                    if (RoadsNearbyInGrid(pos.x, pos.z)) 
                    { 
                        //gridMatrix[pos.x+1, pos.z].NearRoadDirection = NearRoadDirection.West;
                        CreatePavement(pos.x + 1, pos.z, true, true, NearRoadDirection.West);
                    }
                    //Debug.Log("road end left " + pos.x + " " + pos.z);
                    
                }
                else if (neighbourDir.Contains(Direction.Up))
                {
                    rotation = Quaternion.Euler(0, -90, 0);
                    if (RoadsNearbyInGrid(pos.x, pos.z)) 
                    { 
                        //gridMatrix[pos.x, pos.z -1].NearRoadDirection = NearRoadDirection.North;
                        CreatePavement(pos.x, pos.z -1, false, false, NearRoadDirection.North);
                    }
                    //Debug.Log("road end up " + pos.x + " " + pos.z);
                }

                roadDict[pos] = Instantiate(roadEnd, pos, rotation, transform);
                counter++; 
                //Checks if this road end is closer to the center than the previous, if so add to list
                if(((Vector3.Distance(connectorRoadEnd, CityManager.Instance.CenterOfZones) > Vector3.Distance(pos, CityManager.Instance.CenterOfZones)) || connectorRoadEnd == null)
                    && gridMatrix[pos.x,pos.z].Zone.Equals(zone))
                {
                    connectorRoadEnd = pos;
                    //GameObject temp = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Capsule));
                    //temp.transform.position = pos;

                }
                else
                {
                    //GameObject temp = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere));
                    //temp.transform.position = pos;
                    //temp.name = "Sphere " + counter;
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
                    if (RoadsNearbyInGrid(pos.x,pos.z)) 
                    { 
                        //gridMatrix[pos.x - 1, pos.z].NearRoadDirection = NearRoadDirection.East;
                        CreatePavement(pos.x -1, pos.z, true, false, NearRoadDirection.East);
                    }
                    //Debug.Log("Bend down and right " + pos.x + " " + pos.z);
                }
                else if (neighbourDir.Contains(Direction.Left) && neighbourDir.Contains(Direction.Down))
                {
                    rotation = Quaternion.Euler(0, 180, 0);
                    if (RoadsNearbyInGrid(pos.x, pos.z)) 
                    { 
                        //gridMatrix[pos.x + 1, pos.z].NearRoadDirection = NearRoadDirection.West;
                        CreatePavement(pos.x + 1, pos.z, true, true, NearRoadDirection.West);
                    }
                    //Debug.Log("Bend Left and Down " + pos.x + " " + pos.z);
                }
                else if (neighbourDir.Contains(Direction.Up) && neighbourDir.Contains(Direction.Left))
                {
                    rotation = Quaternion.Euler(0, -90, 0);
                    if (RoadsNearbyInGrid(pos.x, pos.z)) 
                    { 
                      //gridMatrix[pos.x + 1, pos.z].NearRoadDirection = NearRoadDirection.West;
                        CreatePavement(pos.x +1, pos.z, true, true, NearRoadDirection.West);
                    }
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
        fixRoadPossibilities.Clear();
        
        //return connectorRoadEnd;
    }

    private bool RoadsNearbyInGrid(int x, int z)
    {
        if((x - 1 >= 0 && x + 1 < CityManager.Instance.X) && (z - 1 >= 0 && z + 1 < CityManager.Instance.X))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RoadEndFixer(List<Vector3> fixes)
    {
        foreach(Vector3 fix in fixes)
        {
            List<Direction> neighbourDir = PlacementHelper.FindNeighbour(Vector3Int.RoundToInt(fix), roadDict.Keys);
            Debug.Log(fix.x + " " + fix.z + " has neighbours " + neighbourDir.Count);

            if(neighbourDir.Count > 1) 
            { 

            }
        }
    }

    private void CreatePavement(int x, int z, bool changeX, bool isIncrease, NearRoadDirection direction)
    {

        //gridMatrix[x, z].Material = CityManager.Instance.pavementMaterial;
        gridMatrix[x, z].TileType = TileType.Pavement;

        if(changeX && x + 1 < CityManager.Instance.X && x - 1 > 0)
        {
            if(isIncrease)
            {
                gridMatrix[x +1, z].NearRoadDirection = direction;
            }
            else
            {
                gridMatrix[x -1, z].NearRoadDirection = direction;
            }
            
        }
        else if(changeX && z + 1 < CityManager.Instance.Z && z - 1 > 0)
        {
            if (isIncrease)
            {
                gridMatrix[x, z+1].NearRoadDirection = direction;
            }
            else
            {
                gridMatrix[x, z-1].NearRoadDirection = direction;
            }
        }

    }

    public void Reset()
    {
        gridMatrix = null;
        roadDict.Clear();
        fixRoadPossibilities.Clear();
        mainRoad = false;

        Debug.Log("Road Dict count" + roadDict.Count);
    }
}
