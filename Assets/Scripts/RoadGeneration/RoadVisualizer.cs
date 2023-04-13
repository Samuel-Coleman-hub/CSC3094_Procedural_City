using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void StartRoadGeneration(Vector3 startPos, CityZone zone)
    {
        this.length = zone.roadLength;
        //Position agent to start in center for road placement
        //roadHelper.transform.position = startPos;
        this.startPos = startPos;
        string sequence = lSystem.GenerateSentence(zone.axiom, zone.rules, zone.iterationLimit, zone.randomIgnoreRuleModifier, zone.chanceToIgnoreRule);
        VisualizeSequence(sequence);
    }

    
    private void VisualizeSequence(string sequence)
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
                    roadHelper.PlaceRoad(tempPos, Vector3Int.RoundToInt(direction), length);
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
        roadHelper.FixRoad();
        
        //roadHelper.transform.position = new Vector3(this.transform.position.x, cityManager.gridCenter.y - 0.15f,
         //   this.transform.position.z);
    }
}
