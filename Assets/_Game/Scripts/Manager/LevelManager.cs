using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public GameObject brickPrefab;
    public GameObject obstaclePrefab;
    public GameObject bridgePrefab;
    public GameObject WinposPrefab;
    public string levelFileName = "Level_1";

    void Start() { GenerateLevel(); }

    public void GenerateLevel()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(levelFileName);
        if (jsonAsset == null) return;

        LevelData data = JsonUtility.FromJson<LevelData>(jsonAsset.text);
        Dictionary<string, Transform> groups = new Dictionary<string, Transform>();

        foreach (TileData t in data.tiles)
        {
            GameObject prefab = null;
            if (t.type == "Bricks") prefab = brickPrefab;
            else if (t.type == "Obstacles") prefab = obstaclePrefab;
            else if (t.type == "Bridges") prefab = bridgePrefab;
            else if (t.type == "Winpos") prefab = WinposPrefab;
            if (prefab != null)
            {
                if (!groups.ContainsKey(t.type))
                {
                    GameObject g = new GameObject(t.type);
                    g.transform.SetParent(this.transform);
                    groups.Add(t.type, g.transform);
                }

                GameObject newTile = Instantiate(prefab, new Vector3(t.x, t.y, t.z), Quaternion.Euler(t.rotX, t.rotY, t.rotZ));
                
                newTile.name = prefab.name;
                newTile.layer = t.layer;
                newTile.tag = t.parentTag;


                newTile.transform.SetParent(groups[t.type]);
                Transform[] children = newTile.GetComponentsInChildren<Transform>(true);
                for (int i = 1; i < children.Length; i++)
                {
                    if (i - 1 < t.childrenTags.Count)
                        children[i].tag = t.childrenTags[i - 1];
                }
                newTile.isStatic = true;
            }
        }
        
    }
}