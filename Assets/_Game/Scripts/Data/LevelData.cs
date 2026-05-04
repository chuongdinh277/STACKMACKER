using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileData
{
    public Vector3 position;
    public Vector3 rotation;
    public TileType type;
    public string parentTag = "Untagged";
    public int layer;
    public List<string> childrenTags = new List<string>();
}

[System.Serializable]
public class LevelData
{
    public Vector3 playerStartPos;
    public List<TileData> tiles = new List<TileData>();
}

[System.Serializable]
public struct PrefabMapping
{
    public TileType type;
    public GameObject prefab;
}