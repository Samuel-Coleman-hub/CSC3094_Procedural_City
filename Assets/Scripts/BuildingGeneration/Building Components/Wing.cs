using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wing
{
    RectInt _bounds;
    Story[] _stories;
    Roof _roof;

    public RectInt Bounds { get { return _bounds; } }
    public Story[] Stories { get { return _stories; } }
    public Roof Roof { get { return _roof; } }

    public Wing(RectInt bounds)
    {
        _bounds = bounds;
    }

    public Wing(RectInt bound, Story[] stories, Roof roof)
    {
        _bounds= bound;
        _stories= stories;
        _roof= roof;
    }

    public override string ToString()
    {
        string wing = "Wing(" + _bounds.ToString() + "):\n";
        foreach(Story s in _stories)
        {
            wing += "\t" + s.ToString() + "\n";
        }
        wing += "\t" + _roof.ToString() + "\n";
        return wing;
    }
}
