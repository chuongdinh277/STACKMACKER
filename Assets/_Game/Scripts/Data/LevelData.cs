using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public float x, y, z;
    public float rotX, rotY, rotZ;
    public string type;
    public string parentTag;
    public int layer;

    public List<string> childrenTags = new List<string>();

}

[System.Serializable]
public class LevelData
{
    public List<TileData> tiles = new List<TileData>();
}