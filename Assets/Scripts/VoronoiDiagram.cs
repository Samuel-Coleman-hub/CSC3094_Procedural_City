using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagram
{
    private Vector2Int[] centroids;
    public Color[] colourRegions;

    public void GenerateVoronoi(int numOfZones, int x, int z)
    {
        centroids = new Vector2Int[numOfZones];
        colourRegions = new Color[numOfZones];
        for (int i = 0; i < numOfZones; i++)
        {
            centroids[i] = new Vector2Int(Random.Range(0, x), Random.Range(0, z));
            colourRegions[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
    }

    public int GetClosestCentroidIndex(Vector2Int pixelPos)
    {
        float closestDst = float.MaxValue;
        int index = 0;
        for (int i = 0; i < centroids.Length; i++)
        {
            if (Vector2.Distance(pixelPos, centroids[i]) < closestDst)
            {
                closestDst = Vector2.Distance(pixelPos, centroids[i]);
                index = i;
            }
        }
        return index;
    }
}
