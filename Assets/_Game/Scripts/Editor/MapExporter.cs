using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class MapExporter : EditorWindow
{
    [MenuItem("StackMaker/Export Current Map")]
    public static void Export()
    {
        GameObject grid = GameObject.Find("Grid");
        if (grid == null)
        {
            Debug.Log("Ko timf thay grid");
            return;
        }

        LevelData leveldata = new LevelData();

        bool foundStart = false;

        foreach (Transform category in grid.transform)
        {
            foreach (Transform item in category)
            {
                if (item.CompareTag("startPoint"))
                {
                    leveldata.playerStartPos = item.position;
                    foundStart = true;
                }

                TileData t = new TileData();
                t.position = item.position;
                t.rotation = item.eulerAngles;
                t.type = category.name;
                t.layer = item.gameObject.layer;
                t.parentTag = item.tag;

                t.childrenTags.Clear();

                foreach (Transform child in item)
                {
                    t.childrenTags.Add(child.tag);
                }

                leveldata.tiles.Add(t);
            }
        }


        if (! foundStart)
        {
            Debug.Log (" khong tim thay startpoint nao");
        }
        string path = EditorUtility.SaveFilePanel("Save file Json", "Assets/Resources", "Level_1", "json");

        if (!string.IsNullOrEmpty(path))
        {
            string json = JsonUtility.ToJson(leveldata, true);
            File.WriteAllText(path, json); 
            AssetDatabase.Refresh();
            Debug.Log($"<color=green><b>SUCCESS:</b></color> Đã xuất Level vào: {path}");
        }
    }
}