using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal;
using Unity.VisualScripting;

public class LevelManager : MonoBehaviour
{
    [Header("Settings")]
    public string levelFileName = "level_1";

    [Header("Prefab Pool")]
    public List<PrefabMapping> prefabPool;

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

        LevelData data = JsonUtility.FromJson<LevelData>(jsonAsset.text);

        Dictionary<string, Transform> groups = new Dictionary<string, Transform>();

        foreach (TileData t in data.tiles)
        {
            GameObject prefab =  prefabPool.Find(p => p.typeName == t.type).prefab;

            if (prefab != null)
            {
                
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
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null && data.playerStartPos != Vector3.zero)
        {
            player.transform.position = data.playerStartPos + Vector3.up * 1.0f;
            Debug.Log("<color=cyan>Player đã vào vị trí xuất phát!</color>");
        } 
    }

    public void LoadNewLevel(string fileName)
    {
        levelFileName = fileName;
        GenerateLevel();
    }
}