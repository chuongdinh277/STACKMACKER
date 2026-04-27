using UnityEngine;
using System.Collections.Generic;


public class LevelManager : MonoBehaviour
{
    [Header("Settings")]
    public string levelFileName = "level_1";

    [Header("Prefab Pool")]
    public List<PrefabMapping> prefabPool;

    public static Dictionary<Vector3, TileData> worldMap = new Dictionary<Vector3, TileData>(); 

    public static List<Vector3> stopPoints = new List<Vector3>();
    void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(levelFileName);

        if (jsonAsset == null)
        {
            Debug.LogError($"[LevelManager] Không tìm thấy file {levelFileName} trong Resources!");
            return;
        }

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        worldMap.Clear();
        LevelData data = JsonUtility.FromJson<LevelData>(jsonAsset.text);

        Dictionary<string, Transform> groups = new Dictionary<string, Transform>();

        foreach (TileData t in data.tiles)
        {
            if (!worldMap.ContainsKey(t.position))
            {
                worldMap.Add(t.position, t);
            }

            var mapping =  prefabPool.Find(p => p.typeName == t.type);
            if (mapping.prefab == null) continue;
            GameObject prefab =  mapping.prefab;

            if (!groups.ContainsKey(t.type))
            {
                GameObject g = new GameObject(t.type);
                g.transform.SetParent(this.transform);
                groups.Add(t.type, g.transform);
            }

            GameObject newTile = Instantiate(prefab, t.position, Quaternion.Euler(t.rotation));

            newTile.name = prefab.name;
            newTile.layer = t.layer;
            newTile.tag = t.parentTag;
            newTile.transform.SetParent(groups[t.type]);

            Transform[] children = newTile.GetComponentsInChildren<Transform>(true);
            for (int i = 1; i < children.Length; i++)
            {
                if (i - 1 < t.childrenTags.Count)
                {
                    children[i].tag = t.childrenTags[i - 1];
                }
            }

            newTile.isStatic = true;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && data.playerStartPos != Vector3.zero)
        {
            player.transform.position = data.playerStartPos + Vector3.up * 1.0f;
            Debug.Log("<color=cyan>Player đã vào vị trí xuất phát!</color>");
        } 
    }


    public static void UpdateTileData (Vector3 pos,  string newType)
    {
        if (worldMap.ContainsKey(pos))
        {
            worldMap[pos].type = newType;
        }
    }


    public static void RemoveTileData (Vector3 pos)
    {
        if (worldMap.ContainsKey(pos))
        {
            worldMap.Remove(pos);
        }
    }

    public void LoadNewLevel(string fileName)
    {
        levelFileName = fileName;
        GenerateLevel();
    }

}