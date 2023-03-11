using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Building,
    Grass,
    Road,
    Empty
}

public class GridTile
{
    private GameObject _tileObject;
    private GameObject _childObject;
    private int _xPosition;
    private int _yPosition;
    private double _centerScore;
    private TileType _type;
    private bool _nearRoad = false;

    public GameObject Object { get { return _tileObject; } set { _tileObject = value; } }
    public GameObject ChildObject { get { return _childObject; } set { _childObject = value; } }
    public int GetX() { return _xPosition; }
    public int GetY() { return _yPosition; }
    public double CenterScore { get { return _centerScore; } set { _centerScore = value; } }
    public TileType TileType { get { return _type; } set { _type = value; } }
    public bool NearRoad { get { return _nearRoad; } set { _nearRoad = value; } }
        

    public GridTile(GameObject tileObject, int xPosition, int yPosition, double centerScore, TileType type)
    {
        _xPosition = xPosition;
        _yPosition = yPosition;
        _centerScore = centerScore;
        _type = type;
        _tileObject = tileObject;

    }
}
