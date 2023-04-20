using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTest : MonoBehaviour
{
    [SerializeField] GameObject wallPrefab;
    [SerializeField] BuildingSettings settings;
    // Start is called before the first frame update
    void Start()
    {
        Building b = BuildingGeneration.Generate(settings);
        GetComponent<BuildingVisualizer>().Visualise(b);
        Debug.Log(b.ToString());

    }
}
