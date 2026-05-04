using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting.Antlr3.Runtime;

public class MapEditorManager : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask groundLayer;
    public Transform gridParent;
    public List<PrefabMapping> prefabPool;
    
    [Header("Save Config")]
    public string levelName = "Level_1";

    [HideInInspector] 
    public TileType selectedType = TileType.Brick;

    [Header("Editor Status")]
    public bool isBrushMode = false; 
    public void SaveCurrentMap(string fullPath)
    {
        LevelData leveldata = new LevelData();
        foreach (Transform category in gridParent) 
        {
            foreach (Transform item in category)
            {
                TileData t = new TileData 
                { 
                    position = item.position, 
                    rotation = item.eulerAngles, 
                    parentTag = item.tag,
                    layer = item.gameObject.layer
                };
                try { t.type = (TileType) System.Enum.Parse(typeof(TileType), category.name); } catch { continue; }
                if (t.type == TileType.StartPoint) leveldata.playerStartPos = item.position;
                leveldata.tiles.Add(t);
            }
        }

       File.WriteAllText(fullPath, JsonUtility.ToJson(leveldata, true));

       levelName = Path.GetFileNameWithoutExtension(fullPath);

       #if UNITY_EDITOR
       UnityEditor.AssetDatabase.Refresh();
       #endif
       Debug.Log("<color=green> Đã lưu map thành công tại:  </color>" + fullPath);
    }

    public void LoadMapFromJson(string json)
    {
        foreach (Transform category in gridParent)
        {
            for (int i = category.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(category.GetChild(i).gameObject);
            }
        }

        LevelData data = JsonUtility.FromJson<LevelData> (json);

        foreach (TileData t in data.tiles)
        {
            var mapping = prefabPool.Find(p => p.type == t.type);
            if (mapping.prefab != null)
            {
                Transform folder = GetFolder(t.type.ToString());
                GameObject newTile = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(mapping.prefab);
                newTile.transform.position = t.position;
                newTile.transform.eulerAngles = t.rotation;
                newTile.layer = t.layer; 
                if (!string.IsNullOrEmpty(t.parentTag))
                {
                    newTile.tag = t.parentTag;
                }

                newTile.transform.SetParent(folder);
            }
        }

    }

    private Transform GetFolder(string name)
    {
        Transform f = gridParent.Find(name);

        if (f == null)
        {
            f = new GameObject(name).transform;
            f.SetParent(gridParent);
        }

        return f;
    }
}