using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum TileType
{
    Building,
    Grass,
    Road,
    Pavement,
    Misc,
    Empty
}

public enum NearRoadDirection
{
    North,
    East,
    South,
    West,
    None
}

public class GridTile
{
    private GameObject _tileObject;
    private GameObject _childObject;
    private int _xPosition;
    private int _zPosition;
    private TileType _type;
    private NearRoadDirection _roadDirection = NearRoadDirection.None;
    private CityZone _zone;
    private MeshRenderer _meshRenderer;

    //Pathfinding variables
    private int _gCost = 0;
    private int _hCost = 0;
    private int _fCost = 0;
    private GridTile _parentNode = null;


    public GameObject Object { get { return _tileObject; } set { _tileObject = value; } }
    public GameObject ChildObject { get { return _childObject; } set { _childObject = value; } }
    public int GetX() { return _xPosition; }
    public int GetZ() { return _zPosition; }
    public TileType TileType { get { return _type; } set { _type = value; } }
    public NearRoadDirection NearRoadDirection { get { return _roadDirection; } set { _roadDirection = value; } }
    public CityZone Zone { get { return _zone; } set { _zone = value; } }

    public int GCost { get { return _gCost; } set { _gCost = value; } }
    public int HCost { get { return _gCost; } set { _gCost = value; } }
    public int FCost { get { return _gCost; } set { _gCost = value; } }
    public GridTile ParentNode { get { return _parentNode; } set { _parentNode = value; } }
    public Material Material { get { return _meshRenderer.material; } set { _meshRenderer.material = value;} }


    public GridTile(GameObject tileObject, int xPosition, int zPosition, TileType type)
    {
        _xPosition = xPosition;
        _zPosition = zPosition;
        _type = type;
        _tileObject = tileObject;
        _meshRenderer = tileObject.GetComponent<MeshRenderer>();
    }

    public void CalculateFScore()
    {
        _fCost = _gCost + _hCost;
    }
}
