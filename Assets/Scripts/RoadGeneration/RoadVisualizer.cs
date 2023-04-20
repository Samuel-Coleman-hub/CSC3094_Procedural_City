using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class RoadVisualizer : MonoBehaviour
{
    [SerializeField] CityManager cityManager;

    public LSystem lSystem;
    List<Vector3> positions = new List<Vector3>();
    public RoadHelper roadHelper;

    //Length and angle that the agent will move at
    private int length = 8;
    private float angle = 90f;

    private Vector3 startPos;
    private CityZone zone;

    //Returns length ensuring always greater than 0
    public int Length
    {
        get
        {
            if (length > 0)
            {
                return length;
            }
            else
            {
                return 1;
            }
        }
        set => length = value;
    }

    public Vector3 StartRoadGeneration(Vector3 startPos, CityZone zone)
    {
        this.length = zone.roadLength;
        this.zone = zone;
        //Position agent to start in center for road placement
        //roadHelper.transform.position = startPos;
        this.startPos = startPos;
        string sequence = lSystem.GenerateSentence(zone.axiom, zone.rules, zone.iterationLimit, zone.randomIgnoreRuleModifier, zone.chanceToIgnoreRule);
        return VisualizeSequence(sequence);
    }

    
    private Vector3 VisualizeSequence(string sequence)
    {
        //Used for saving and loading agents position
        Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
        //var currentPos = cityManager.gridCenter; 
        var currentPos = startPos;

        Vector3 direction = Vector3.forward;
        Vector3 tempPos = Vector3.zero;

        positions.Add(currentPos);

        foreach (char letter in sequence)
        {
            //Map char to enum
            EncodingLetters encoding = (EncodingLetters)letter;
            switch (encoding)
            {
                case EncodingLetters.save:
                    //Add current position variables to 
                    savePoints.Push(new AgentParameters
                    {
                        position = currentPos,
                        direction = direction,
                        length = Length
                    });
                    break;
                case EncodingLetters.load:
                    if (savePoints.Count > 0)
                    {
                        var agentParameter = savePoints.Pop();
                        currentPos = agentParameter.position;
                        direction = agentParameter.direction;
                        Length = agentParameter.length;
                    }
                    else
                    {
                        throw new System.Exception("Don't have saved point in our stack");
                    }
                    break;
                case EncodingLetters.draw:
                    tempPos = currentPos;
                    currentPos += direction * Length;
                    //Used to be drawline
                    roadHelper.PlaceRoad(tempPos, Vector3Int.RoundToInt(direction), length, zone);
                    //Makes line shorter overtime so roads get shorter further we go
                    Length -= 1;
                    positions.Add(currentPos);
                    break;
                case EncodingLetters.turnRight:
                    direction = Quaternion.AngleAxis(angle, Vector3.up) * direction;
                    break;
                case EncodingLetters.turnLeft:
                    direction = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
                    break;
            }
        }
        return roadHelper.FixRoad(zone);
        
        //roadHelper.transform.position = new Vector3(this.transform.position.x, cityManager.gridCenter.y - 0.15f,
         //   this.transform.position.z);
    }

    public void ConnectZones(List<CityZone> zones)
    {
        GameObject center = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cylinder));
        center.transform.position = cityManager.centerOfZones;
        Debug.Log("Spawned cylinder at the center of all city zones " + center.transform.position.x + " " + center.transform.position.z);
        center.name = "Brucey Center";


        Pathfinding pathfinder = new Pathfinding();

        GridTile centerTile = cityManager.gridMatrix[(int)cityManager.centerOfZones.x, (int)cityManager.centerOfZones.z];
        GridTile roadEndTile = cityManager.gridMatrix[(int)zones[0].roadEndToCenter.x, (int)zones[0].roadEndToCenter.z];
        List<GridTile> firstRoad = pathfinder.FindPath(roadEndTile, centerTile, cityManager.gridMatrix, cityManager.x, cityManager.z);

        roadEndTile = cityManager.gridMatrix[(int)zones[1].roadEndToCenter.x, (int)zones[1].roadEndToCenter.z];
        List<GridTile> secondRoad = pathfinder.FindPath(roadEndTile, centerTile, cityManager.gridMatrix, cityManager.x, cityManager.z);

        List<GridTile> mainRoad = firstRoad.Concat(secondRoad).ToList();

        List<Vector3> fixes = new List<Vector3>();

        fixes.Add(zones[0].roadEndToCenter);
        fixes.Add(zones[1].roadEndToCenter);
        Debug.Log("Added positions " + zones[0].roadEndToCenter.x + " " + zones[0].roadEndToCenter.z + " and " +zones[1]
            .roadEndToCenter.x + " " + zones[1].roadEndToCenter.z);

        //Find paths for other zones
        for (int i = 2; i < zones.Count; i++)
        {
            //Vector3 closestTile = new Vector3(cityManager.x *2, 0f, cityManager.z *2);

            //Pick a position from the main road
            //foreach (GridTile tile in mainRoad)
            //{
            //    Vector3 tilePos = new Vector3(tile.GetX(), 0f, tile.GetY());
            //    if (Vector3.Distance(tilePos, zones[i].roadEndToCenter) < Vector3.Distance(closestTile, zones[i].roadEndToCenter))
            //    {
            //        closestTile = tilePos;
            //    }
            //}

            //Generate path
            GridTile roadEnd = cityManager.gridMatrix[(int)zones[i].roadEndToCenter.x, (int)zones[i].roadEndToCenter.z];
            //GridTile endTile = cityManager.gridMatrix[(int)closestTile.x, (int)closestTile.z];
            List<GridTile> roadTiles = pathfinder.FindPath(roadEnd, centerTile, cityManager.gridMatrix, cityManager.x, cityManager.z);

            //Add to list of existing paths
            mainRoad = mainRoad.Concat(roadTiles).ToList();
        }

        //mainRoad = mainRoad.Distinct().ToList();

        //Place all of the road assets
        for (int i = 0; i < mainRoad.Count(); i++)
        {
            Vector3 previous;
            if (i - 1 < 0) 
            { 
                previous = new Vector3(mainRoad[i].Zone.roadEndToCenter.x, 0f, mainRoad[i].Zone.roadEndToCenter.z);
            }
            else
            {
                previous = new Vector3(mainRoad[i-1].GetX(), 0f, mainRoad[i-1].GetY());
            }

            Vector3 current = new Vector3(mainRoad[i].GetX(), 0f, mainRoad[i].GetY());

            var head = previous - current;
            var dist = head.magnitude;
            var dir = head / dist;
            roadHelper.PlaceRoad(current, Vector3Int.RoundToInt(dir), 1);
        }

        List<Vector3> roadPos = new List<Vector3>();
        foreach (GridTile tile in mainRoad)
        { 
            roadPos.Add(new Vector3(tile.GetX(), 0f, tile.GetY()));
        }

        roadHelper.FixRoad(zone);


        //for (int i = 2; i < zones.Count; i++)
        //{
        //    GridTile destinationTile;
        //    GridTile roadEndTile = cityManager.gridMatrix[(int)zones[i].roadEndToCenter.x, (int)zones[i].roadEndToCenter.z];
        //    if (i <= 1)
        //    {
        //        destinationTile = centerTile;
        //    }
        //    else
        //    {
        //        destinationTile = mainRoad[Random.Range(0, mainRoad.Count)];
        //    }

        //    List<GridTile> path = pathfinder.FindPath(roadEndTile, destinationTile, cityManager.gridMatrix, cityManager.x, cityManager.z);
            
        //    foreach (GridTile tile in path)
        //    {
        //        tile.Object.GetComponent<MeshRenderer>().material.color = Color.black;
        //    }
        //}
        
    }
}
