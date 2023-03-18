using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagram : MonoBehaviour
{
    [SerializeField] CityManager cityManager;

    public Vector2Int imageSize;
    public int regionAmount;

    List<Texture2D> imageTiles;

    private GridTile[,] gridMatrix;

    private void Start()
    {
        gridMatrix = cityManager.gridMatrix;
        //Split(GetDiagram(), 120, 120);
        Sprite sprite = Sprite.Create(
            GetDiagram(), new Rect(0, 0, imageSize.x, imageSize.y), Vector2.one * 0.5f);
        GetComponent<SpriteRenderer>().sprite = sprite;
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
                //Material mat = new Material(Shader.Find("Specular"));
                gridMatrix[x, y].Object.GetComponent<MeshRenderer>().material.color = regions[GetClosestCentroidIndex(new Vector2Int(x, y), centroids)];
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

    //FROM UNITY FORUMN
    //public void Split(Texture2D image, int width, int height)
    //{
    //    bool perfectWidth = image.width % width == 0;
    //    bool perfectHeight = image.height % height == 0;

    //    int lastWidth = width;
    //    if (!perfectWidth)
    //    {
    //        lastWidth = image.width - ((image.width / width) * width);
    //    }

    //    int lastHeight = height;
    //    if (!perfectHeight)
    //    {
    //        lastHeight = image.height - ((image.height / height) * height);
    //    }

    //    int widthPartsCount = image.width / width + (perfectWidth ? 0 : 1);
    //    int heightPartsCount = image.height / height + (perfectHeight ? 0 : 1);

    //    for (int i = 0; i < widthPartsCount; i++)
    //    {
    //        for (int j = 0; j < heightPartsCount; j++)
    //        {
    //            int tileWidth = i == widthPartsCount - 1 ? lastWidth : width;
    //            int tileHeight = j == heightPartsCount - 1 ? lastHeight : height;

    //            Texture2D g = new Texture2D(tileWidth, tileHeight);
    //            g.SetPixels(image.GetPixels(i * width, j * height, tileWidth, tileHeight));
    //            g.Apply();
    //            imageTiles.Add(g);
    //        }
    //    }
    //}



}
