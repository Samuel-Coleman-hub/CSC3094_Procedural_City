using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    private const int STRAIGHT_COST = 10;
    private const int DIAGONAL_COST = 24;

    private List<GridTile> openList;
    private HashSet<GridTile> closedList;

    private GridTile[,] gridMatrix;
    private int width;
    private int height;

    public List<GridTile> FindPath(GridTile startTile, GridTile endTile, GridTile[,] gridMatrix, int x, int z)
    {
        width = x - 1;
        height = z - 1;
        this.gridMatrix = gridMatrix;
        openList = new List<GridTile> { startTile };
        closedList = new HashSet<GridTile>();

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                gridMatrix[i,j].GCost = int.MaxValue;
                gridMatrix[i,j].CalculateFScore();
                gridMatrix[i,j].ParentNode = null;
            }
        }

        startTile.GCost= 0;
        startTile.HCost = CalculateDistance(startTile, endTile);
        startTile.CalculateFScore();

        while(openList.Count > 0)
        {
            GridTile currentTile = openList.OrderBy( x => x.FCost).FirstOrDefault();
            if(currentTile == endTile)
            {
                //Found end tile so return path
                return CalculatePath(endTile);
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach(GridTile neighbourTile in GetNeighbourList(currentTile)) 
            {
                if (closedList.Contains(neighbourTile))
                {
                    continue;
                }

                int tempGCost = currentTile.GCost + CalculateDistance(currentTile, neighbourTile);
                if(tempGCost < neighbourTile.GCost) 
                {
                    neighbourTile.ParentNode = currentTile;
                    neighbourTile.GCost = tempGCost;
                    neighbourTile.HCost = CalculateDistance(neighbourTile, endTile);
                    neighbourTile.CalculateFScore();

                    if (!openList.Contains(neighbourTile))
                    {
                        openList.Add(neighbourTile);
                    }
                }
            }
        }
        return null;
    }

    private List<GridTile> GetNeighbourList(GridTile currentTile)
    {
        List<GridTile> neighbourList = new List<GridTile>();

        //Left
        if(currentTile.GetX() - 1 >= 0)
        {
            neighbourList.Add(gridMatrix[currentTile.GetX() - 1, currentTile.GetZ()]);
        }

        //Right
        if (currentTile.GetX() + 1 < width)
        {
            neighbourList.Add(gridMatrix[currentTile.GetX() + 1, currentTile.GetZ()]);
        }

        //Down
        if (currentTile.GetZ() - 1 >= 0)
        {
            neighbourList.Add(gridMatrix[currentTile.GetX(), currentTile.GetZ() - 1]);
        }

        //Up
        if(currentTile.GetZ() + 1 < height)
        {
            neighbourList.Add(gridMatrix[currentTile.GetX(), currentTile.GetZ() + 1]);
        }

        return neighbourList;
    }

    private List<GridTile> CalculatePath(GridTile endTile)
    {
        List<GridTile> path = new List<GridTile>();
        path.Add(endTile);
        GridTile currentTile = endTile;
        while (currentTile.ParentNode!= null)
        {
            path.Add(currentTile.ParentNode);
            currentTile = currentTile.ParentNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistance(GridTile a, GridTile b)
    {
        int xDistance = Mathf.Abs(a.GetX() - b.GetX());
        int yDistance = Mathf.Abs(a.GetZ() - b.GetZ());
        int remaining = Mathf.Abs(xDistance - yDistance);

        return DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + STRAIGHT_COST * remaining;
    }
}
