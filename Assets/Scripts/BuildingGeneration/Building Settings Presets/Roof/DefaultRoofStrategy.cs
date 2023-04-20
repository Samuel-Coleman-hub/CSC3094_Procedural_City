using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultRoofStrategy : RoofStratergy
{
    public override Roof GenerateRoof(BuildingSettings settings, RectInt bounds)
    {
        return new Roof();
    }
}
