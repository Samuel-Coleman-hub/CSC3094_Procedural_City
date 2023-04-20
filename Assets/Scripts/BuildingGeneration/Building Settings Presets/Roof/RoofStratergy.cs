using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoofStratergy : ScriptableObject
{
    public abstract Roof GenerateRoof(BuildingSettings settings, RectInt bounds);
}
