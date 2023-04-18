using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Story
{
    int _level;
    Wall[] _walls;

    public int Level { get { return _level; } }
    public Wall[] Walls { get { return _walls; } }

    public Story(int level, Wall[] walls)
    {
        _level=level;
        _walls=walls;
    }

    public override string ToString()
    {
        string story = "Story " + _level + ":\n";
        story += "\t\tWalls: ";
        foreach(Wall wall in _walls) 
        {
            story += wall.ToString() + ", "; 
        }
        return story;
    }
}
