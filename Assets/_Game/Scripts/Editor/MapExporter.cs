using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
public class MapExporter : EditorWindow
{
    [MenuItem("StackMaker/Export Current Map")]
    public static void Export()
    {
        GameObject grid = GameObject.Find("Grid");

        if (grid == null)
        {
            Debug.Log("false to find grid");
        }

        LevelData levelData = new LevelData();

        foreach (Transform category in grid.transform)
        {
            if (category.name == "Ground 2") continue;

            foreach (Transform item in category)
            {
                TileData t = new TileData();
                t.x = Mathf.Floor(item.position.x) + 0.5f;
                t.y = item.position.y;
                t.z = Mathf.Floor(item.position.z) + 0.5f;
                t.rotX = item.eulerAngles.x;
                t.rotY = item.eulerAngles.y;
                t.rotZ = item.eulerAngles.z;
                t.type = category.name;
                t.layer = item.gameObject.layer;
                
                t.parentTag = item.tag;

                Transform[] allChildren = item.GetComponentsInChildren<Transform>(true);
                for (int i = 1; i < allChildren.Length; i++) 
                {
                    t.childrenTags.Add(allChildren[i].tag);
                }

                levelData.tiles.Add(t);
            }
        }

        string json = JsonUtility.ToJson(levelData, true);
        string folderPath = Application.dataPath + "/Resources";
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        File.WriteAllText(folderPath + "/Level_1.json", json);
        AssetDatabase.Refresh();
        
        Debug.Log($"<color=yellow><b>SUCCESS:</b></color> Đã xuất {levelData.tiles.Count} ô vào Resources/Level_1.json");
    }

}
