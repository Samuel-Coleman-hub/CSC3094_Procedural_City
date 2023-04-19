using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Building b = BuildingGeneration.Generate();
        GetComponent<BuildingVisualizer>().Visualise(b);
        Debug.Log(b.ToString());
        
    }
}
