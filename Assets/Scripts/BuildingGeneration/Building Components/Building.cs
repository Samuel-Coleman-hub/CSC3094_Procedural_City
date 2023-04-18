using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    Vector2Int _size;
    Wing[] _wings;

    public Vector2Int Size { get { return _size; } }
    public Wing[] Wings { get { return _wings; } }

    public Building(int XSize, int YSize, Wing[] wings) 
    {
        _size = new Vector2Int(XSize, YSize);
        _wings = wings;
    }

    //Can print so that we can understand why the building is doing what its doing
    public override string ToString()
    {
        string building = "Building:(" + Size.ToString() + ";" + _wings.Length + ")\n";
        foreach(Wing x in _wings)
        {
            building += "\t" + x.ToString() + "\n";
        }
        return building;
    }
}
