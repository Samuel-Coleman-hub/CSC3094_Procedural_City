using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class BuildingGeneration
{
    public static Building Generate(BuildingSettings settings)
    {
        return new Building(settings.Size.x, settings.Size.y, GenerateWings(settings));
    }

    static Wing[] GenerateWings(BuildingSettings settings)
    {
        //Wing[] wings = new Wing[settings.numberOfWings];
        //for(int i = 0; i < wings.Length; i++)
        //{
        //    wings[i] = Generate(Wing)
        //}
        return new Wing[] { GenerateWing(settings, new RectInt(0, 0, settings.Size.x, settings.Size.y), settings.buildingHeight)};
    }

    static Wing GenerateWing(BuildingSettings settings, RectInt bounds, int numberOfStories)
    {
        return new Wing(
            bounds,
            GenerateStories(settings, bounds, numberOfStories),
            GenerateRoof(settings, bounds)
            );
    }

    static Story[] GenerateStories(BuildingSettings settings, RectInt bounds, int numberOfStories)
    {
        Story[] stories = new Story[numberOfStories];
        for (int i = 0; i < numberOfStories; i++)
        {
            stories[i] = GenerateStory(settings, bounds, i);
        }
        return stories;
    }

    static Story GenerateStory(BuildingSettings settings, RectInt bounds, int level)
    {
        return new Story(level, GenerateWalls(settings, bounds, level));
    }

    static Wall[] GenerateWalls(BuildingSettings settings, RectInt bounds, int level)
    {
        return new Wall[(bounds.size.x + bounds.size.y) * 2];
    }

    static Roof GenerateRoof(BuildingSettings settings, RectInt bounds)
    {
        return new Roof();
    }



}
