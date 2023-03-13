using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagram : MonoBehaviour
{
    public Vector2Int imageSize;
    public int regionAmount;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(
            GetDiagram(), new Rect(0,0,imageSize.x, imageSize.y), Vector2.one * 0.5f);
    }

    private Texture2D GetDiagram()
    {
        //Points we are dividing area with 
        Vector2Int[] centroids = new Vector2Int[regionAmount];
        Color[] regions = new Color[regionAmount];
        for(int i = 0; i < regionAmount; i++)
        {
            centroids[i] = new Vector2Int(Random.Range(0, imageSize.x), Random.Range(0, imageSize.y));
            regions[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);

        }
        Color[] pixelColors = new Color[imageSize.x * imageSize.y];
        for(int x = 0; x < imageSize.x; x++)
        {
            for(int y = 0; y < imageSize.y; y++)
            {
                int index = x * imageSize.x + y;
                pixelColors[index] = regions[GetClosestCentroidIndex(new Vector2Int(x, y), centroids)];
            }
        }
        return GetImageFromColourArray(pixelColors);
    }

    private int GetClosestCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroids)
    {
        float closestDst = float.MaxValue;
        int index = 0;
        for(int i = 0; i < centroids.Length; i++)
        {
            if(Vector2.Distance(pixelPos, centroids[i]) < closestDst)
            {
                closestDst = Vector2.Distance(pixelPos, centroids[i]);
                index = i;
            }
        }
        return index;
    }

    private Texture2D GetImageFromColourArray(Color[] pixelColors)
    {
        Texture2D texture = new Texture2D(imageSize.x, imageSize.y);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(pixelColors);
        texture.Apply();
        return texture;
    }



}
