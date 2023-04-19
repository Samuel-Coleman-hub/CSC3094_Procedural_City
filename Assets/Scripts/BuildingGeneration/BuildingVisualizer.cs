using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingVisualizer : MonoBehaviour
{
    public Transform floorPrefab;
    public Transform[] wallPrefab;
    public Transform[] roofPrefab;
    private Transform buildingEmpty;

    public void Visualise(Building building)
    {
        buildingEmpty = new GameObject("Building").transform;
        foreach(Wing wing in building.Wings)
        {
            VisualiseWing(wing);
        }
    }

    private void VisualiseWing(Wing wing)
    {
        Transform wingEmpty = new GameObject("Wing").transform;
        wingEmpty.SetParent(buildingEmpty);
        foreach(Story story in wing.Stories)
        {
            VisualiseStory(story, wing, wingEmpty);
        }
        VisualiseRoof(wing, wingEmpty);
    }

    private void VisualiseStory(Story story, Wing wing, Transform wingEmpty)
    {
        Transform storyEmpty = new GameObject("Story" + story.Level).transform;
        storyEmpty.SetParent(wingEmpty);
        for(int x = wing.Bounds.min.x; x < wing.Bounds.max.x; x++)
        {
            for(int y = wing.Bounds.min.y; y < wing.Bounds.max.y; y++)
            {
                PlaceFloor(x, y, story.Level, storyEmpty);

                //Place South Wall
                if(y == wing.Bounds.min.y)
                {
                    Transform wall = wallPrefab[(int)story.Walls[x-wing.Bounds.min.x]];
                    PlaceSouthWall(x, y, story.Level, storyEmpty, wall);
                }

                //Place East Wall
                if(x == wing.Bounds.min.x + wing.Bounds.size.x - 1)
                {
                    Transform wall = wallPrefab[(int)story.Walls[wing.Bounds.size.x + y - wing.Bounds.min.y]];
                    PlaceEastWall(x, y, story.Level, storyEmpty, wall);
                }

                //Place North Wall
                if(y == wing.Bounds.min.y + wing.Bounds.size.y - 1)
                {
                    Transform wall = wallPrefab[(int)story.Walls[wing.Bounds.size.x * 2 + wing.Bounds.size.y - (x - wing.Bounds.min.x + 1)]];
                    PlaceNorthWall(x, y, story.Level, storyEmpty, wall);
                }

                //Place West Wall
                if(x == wing.Bounds.min.x)
                {
                    Transform wall = wallPrefab[(int)story.Walls[(wing.Bounds.size.x + wing.Bounds.size.y)  * 2  - (y - wing.Bounds.min.y +1)]];
                    PlaceWestWall(x, y, story.Level, storyEmpty, wall);
                }
            }
        }

    }

    private void PlaceFloor(int x, int y, int level, Transform storyEmpty)
    {
        Transform f = Instantiate(floorPrefab, storyEmpty.TransformPoint(new Vector3(x * -3f, 0f + level * 2.5f, y * -3f)), Quaternion.identity);
        f.SetParent(storyEmpty);
    }

    private void PlaceSouthWall(int x, int y, int level, Transform storyEmpty, Transform wall)
    {
        Transform w = Instantiate(wall, storyEmpty.TransformPoint(new Vector3(x * -3f, 0.3f + level * 2.5f, y * 3f - 0.5f)), Quaternion.Euler(0f, 90f, 0f));
        w.SetParent(storyEmpty);
    }

    private void PlaceEastWall(int x, int y, int level, Transform storyEmpty, Transform wall)
    {
        Transform w = Instantiate(wall, storyEmpty.TransformPoint(new Vector3(x * -3f - 2.5f, 0.3f + level * 2.5F, y * -3f)), Quaternion.identity);
        w.SetParent(storyEmpty);
    }

    private void PlaceNorthWall(int x, int y, int level, Transform storyEmpty, Transform wall)
    {
        Transform w = Instantiate(wall, storyEmpty.TransformPoint(new Vector3(x * -3f, 0.3f + level * 2.5f, y * -3f - 3f)), Quaternion.Euler(0f, 90f, 0f));
        w.SetParent(storyEmpty);
    }

    private void PlaceWestWall(int x, int y, int level, Transform storyEmpty, Transform wall)
    {
        Transform w = Instantiate(wall, storyEmpty.TransformPoint(new Vector3(x * -3f, 0.3f + level * 2.5f, y * -3f)), Quaternion.identity);
        w.SetParent(storyEmpty);
    }

    //Code from board to bits but I will need to change to work for mine
    private void VisualiseRoof(Wing wing, Transform wingFolder)
    {
        for (int x = wing.Bounds.min.x; x < wing.Bounds.max.x; x++)
        {
            for (int y = wing.Bounds.min.y; y < wing.Bounds.max.y; y++)
            {
                PlaceRoof(x, y, wing.Stories.Length, wingFolder, wing.Roof.Type, wing.Roof.Direction);
            }
        }
    }

    private void PlaceRoof(int x, int y, int level, Transform wingFolder, RoofType type, RoofDirection direction)
    {
        Transform r;
        r = Instantiate(
            roofPrefab[(int)type],
            wingFolder.TransformPoint(
                new Vector3(
                        x * -3f + rotationOffset[(int)direction].x,
                        level * 2.5f + (type == RoofType.Point ? -0.3f : 0f),
                        y * -3f + rotationOffset[(int)direction].z
                    )
                ),
            Quaternion.Euler(0f, rotationOffset[(int)direction].y, 0f)
            );
        r.SetParent(wingFolder);
    }

    Vector3[] rotationOffset = {
        new Vector3 (-3f, 270f, 0f),
        new Vector3 (0f, 0f, 0f),
        new Vector3 (0f, 90, -3f),
        new Vector3 (-3f, 180, -3f)
    };

}
