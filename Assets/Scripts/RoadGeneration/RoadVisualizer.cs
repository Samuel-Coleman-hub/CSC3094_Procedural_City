using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class RoadVisualizer : MonoBehaviour
{

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

    public Vector3 GenerateRoad(Vector3 startPos, CityZone zone)
    {
        this.length = zone.roadLength;
        this.zone = zone;
        this.startPos = startPos;
        string sequence = lSystem.GenerateSentence(zone.axiom, zone.rules, zone.iterationLimit, zone.randomIgnoreRuleModifier, zone.chanceToIgnoreRule);
        return VisualizeSequence(sequence);
    }

    
    private Vector3 VisualizeSequence(string sequence)
    {
        //Used for saving and loading agents position
        Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
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
                    Debug.Log("Road length at placement " + length);
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
        //return roadHelper.FixRoad(zone);
        return zone.roadEndToCenter;
        
    }

    public void ConnectZones(List<CityZone> zones)
    {
        Pathfinding pathfinder = new Pathfinding();

        GridTile centerTile = CityManager.Instance.GridMatrix[(int)CityManager.Instance.CenterOfZones.x, (int)CityManager.Instance.CenterOfZones.z];
        GridTile roadEndTile = CityManager.Instance.GridMatrix[(int)zones[0].roadEndToCenter.x, (int)zones[0].roadEndToCenter.z];
        List<GridTile> firstRoad = pathfinder.FindPath(roadEndTile, centerTile, CityManager.Instance.GridMatrix, CityManager.Instance.X, CityManager.Instance.Z);

        roadEndTile = CityManager.Instance.GridMatrix[(int)zones[1].roadEndToCenter.x, (int)zones[1].roadEndToCenter.z];
        List<GridTile> secondRoad = pathfinder.FindPath(roadEndTile, centerTile, CityManager.Instance.GridMatrix, CityManager.Instance.X, CityManager.Instance.Z);

        List<GridTile> mainRoad = firstRoad.Concat(secondRoad).ToList();

        List<Vector3> fixes = new List<Vector3>
        {
            zones[0].roadEndToCenter,
            zones[1].roadEndToCenter
        };

        //Find paths for other zones
        for (int i = 2; i < zones.Count; i++)
        {
            //Generate path
            GridTile roadEnd = CityManager.Instance.GridMatrix[(int)zones[i].roadEndToCenter.x, (int)zones[i].roadEndToCenter.z];
            fixes.Add(new Vector3((int)zones[i].roadEndToCenter.x, 0, (int)zones[i].roadEndToCenter.z));
            //GridTile endTile = CityManager.Instance.gridMatrix[(int)closestTile.x, (int)closestTile.z];
            List<GridTile> roadTiles = pathfinder.FindPath(roadEnd, centerTile, CityManager.Instance.GridMatrix, CityManager.Instance.X, CityManager.Instance.Z);

            //Add to list of existing paths
            mainRoad = mainRoad.Concat(roadTiles).ToList();
        }

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
                previous = new Vector3(mainRoad[i-1].GetX(), 0f, mainRoad[i-1].GetZ());
            }

            Vector3 current = new Vector3(mainRoad[i].GetX(), 0f, mainRoad[i].GetZ());

            var head = previous - current;
            var dist = head.magnitude;
            var dir = head / dist;
            roadHelper.PlaceRoad(current, Vector3Int.RoundToInt(dir), 1);
        }

        List<Vector3> roadPos = new List<Vector3>();
        foreach (GridTile tile in mainRoad)
        { 
            roadPos.Add(new Vector3(tile.GetX(), 0f, tile.GetZ()));
        }

        
        //StartCoroutine(fix());
        roadHelper.FixRoad(zone);

    }

    public void Reset()
    {
        positions = new List<Vector3>();
        roadHelper.Reset();
    }
}
