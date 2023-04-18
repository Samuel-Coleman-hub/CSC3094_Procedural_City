using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roof
{
    RoofType _type;
    RoofDirection _direction;

    public RoofType Type { get { return _type; } }
    public RoofDirection Direction { get { return _direction; } }

    public Roof(RoofType type = RoofType.Peak, RoofDirection direction = RoofDirection.North)
    {
        _type=type;
        _direction=direction;
    }

    public override string ToString()
    {
        return "Roof: " + _type.ToString() + ((_type == RoofType.Peak || _type == RoofType.Slope) ? ", " + _direction.ToString() : ""); 
    }
}

public enum RoofType
{
    Point,
    Peak,
    Slope,
    Flat
}

public enum RoofDirection
{
    North, //Positive Y
    East, //Positive X
    South, // Negative Y
    West // Negative X
}
