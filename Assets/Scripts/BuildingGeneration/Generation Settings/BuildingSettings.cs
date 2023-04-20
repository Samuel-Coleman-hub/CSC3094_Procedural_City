using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Building Generation/Building Settings")]

public class BuildingSettings : ScriptableObject
{
    public Vector2Int buildingSize;

    public int buildingHeight;

    public Vector2Int Size { get { return buildingSize; } }
}
