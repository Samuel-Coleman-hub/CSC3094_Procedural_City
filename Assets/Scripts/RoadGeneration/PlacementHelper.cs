using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlacementHelper
{
    //Looks at neighbours to determine the type of road it needs to place
    public static List<Direction> FindNeighbour(Vector3Int pos, ICollection<Vector3Int> collection)
    {
        List<Direction> neighbourDirections = new List<Direction>();

        //Checks to the right
        if(collection.Contains(pos + Vector3Int.right))
        {
            neighbourDirections.Add(Direction.Right);
        }
        if (collection.Contains(pos - Vector3Int.right))
        {
            neighbourDirections.Add(Direction.Left);
        }
        if (collection.Contains(pos + new Vector3Int(0,0,1)))
        {
            neighbourDirections.Add(Direction.Up);
        }
        if (collection.Contains(pos - new Vector3Int(0, 0, 1)))
        {
            neighbourDirections.Add(Direction.Down);
        }

        return neighbourDirections;
    }
}
