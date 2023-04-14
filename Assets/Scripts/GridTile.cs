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
    private CityZone _zone;

    //Pathfinding variables
    private int _gCost = 0;
    private int _hCost = 0;
    private int _fCost = 0;
    private GridTile _parentNode = null;


    public GameObject Object { get { return _tileObject; } set { _tileObject = value; } }
    public GameObject ChildObject { get { return _childObject; } set { _childObject = value; } }
    public int GetX() { return _xPosition; }
    public int GetY() { return _yPosition; }
    public double CenterScore { get { return _centerScore; } set { _centerScore = value; } }
    public TileType TileType { get { return _type; } set { _type = value; } }
    public bool NearRoad { get { return _nearRoad; } set { _nearRoad = value; } }
    public CityZone Zone { get { return _zone; } set { _zone = value; } }

    public int GCost { get { return _gCost; } set { _gCost = value; } }
    public int HCost { get { return _gCost; } set { _gCost = value; } }
    public int FCost { get { return _gCost; } set { _gCost = value; } }
    public GridTile ParentNode { get { return _parentNode; } set { _parentNode = value; } }


    public GridTile(GameObject tileObject, int xPosition, int yPosition, double centerScore, TileType type)
    {
        _xPosition = xPosition;
        _yPosition = yPosition;
        _centerScore = centerScore;
        _type = type;
        _tileObject = tileObject;

    }

    public void CalculateFScore()
    {
        _fCost = _gCost + _hCost;
    }
}
