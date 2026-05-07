using UnityEngine;
using UnityEditor;
using System.IO;
[CustomEditor(typeof(MapEditorManager))]
public class MapEditorTool : Editor
{
    private MapEditorManager manager;
    private void OnEnable() => manager = (MapEditorManager)target;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space(10);

        GUI.backgroundColor = manager.isBrushMode ? Color.red : Color.green;
        if (GUILayout.Button(manager.isBrushMode ? "TẮT BÚT VẼ " : "BẬT BÚT VẼ ", GUILayout.Height(40)))
        {
            manager.isBrushMode = !manager.isBrushMode;
        }
        GUI.backgroundColor = Color.white;

        if (manager.isBrushMode)
        {
            EditorGUILayout.HelpBox("ĐANG Ở CHẾ ĐỘ BÚT: Click và Di chuột để vẽ. Giữ Shift để xóa.", MessageType.Info);
            EditorGUILayout.LabelField("---------- CHỌN LOẠI GẠCH ----------", EditorStyles.boldLabel);
            foreach (TileType type in System.Enum.GetValues(typeof(TileType)))
            {
                if (type == TileType.None) continue;
                GUI.backgroundColor = (manager.selectedType == type) ? Color.cyan : Color.white;
                if (GUILayout.Button(type.ToString())) manager.selectedType = type;
            }
        }
        
        

        EditorGUILayout.Space(10);
        GUI.backgroundColor = Color.yellow;

        if (GUILayout.Button("OPEN LEVEL FILE", GUILayout.Height(35)))
        {
            string path = EditorUtility.OpenFilePanel("Chọn file Level Json", Application.dataPath + "/Resources", "json");

            if (!string.IsNullOrEmpty(path))
            {
                string jsonContent = File.ReadAllText(path);

                manager.levelName = Path.GetFileNameWithoutExtension(path);

                manager.LoadMapFromJson(jsonContent);
            }
        }


        EditorGUILayout.Space(20);
        if (GUILayout.Button("SAVE MAP (JSON)", GUILayout.Height(30)))
        {
            string defaultPath = Application.dataPath + "/Resources";
            string path = EditorUtility.SaveFilePanel("Lưu level Game", defaultPath, manager.levelName, "json");

            if (!string.IsNullOrEmpty(path))
            {
                manager.SaveCurrentMap(path);
            }
        }
    }

    private void OnSceneGUI()
    {
        if (manager == null || !manager.isBrushMode) return;

        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(controlID);

        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, manager.groundLayer))
        {
            Vector3 spawnPos = new Vector3(Mathf.Round(hit.point.x), 0, Mathf.Round(hit.point.z));
            
            Handles.color = e.shift ? Color.red : Color.cyan;
            Handles.DrawWireCube(spawnPos, Vector3.one * 1.1f);
            Handles.DrawSolidDisc(spawnPos, Vector3.up, 0.2f);

            if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
            {
                if (e.shift) RemoveTile(spawnPos);
                else CreateTile(spawnPos);
                e.Use();
            }
        }
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            manager.isBrushMode = false;
            Repaint();
        }
        
        SceneView.RepaintAll();
    }
    private void CreateTile(Vector3 pos) 
    {
        foreach (Transform cat in manager.gridParent)
        {
            foreach (Transform t in cat)
            {
                if (Vector3.Distance(t.position, pos) < 0.1f) 
                {
                    return;
                }
            } 
        }
        var mapping = manager.prefabPool.Find(p => p.type == manager.selectedType);

        if (mapping.prefab != null) 
        {
            GameObject newTile = (GameObject)PrefabUtility.InstantiatePrefab(mapping.prefab);

            newTile.transform.position = pos;

            newTile.transform.parent = GetFolder(manager.selectedType.ToString());

            Undo.RegisterCreatedObjectUndo(newTile, "Create Tile");
        }
    }
    private void RemoveTile(Vector3 pos) 
    {
        foreach (Transform cat in manager.gridParent)
        {
            foreach (Transform t in cat)
            {
                if (Vector3.Distance(t.position, pos) < 0.1f) 
                { 
                    Undo.DestroyObjectImmediate(t.gameObject); 
                    return; 
                }
            }
                
        }
            
    }
    private Transform GetFolder(string n) 
    {
        Transform f = manager.gridParent.Find(n);
        if (f == null) { f = new GameObject(n).transform; f.SetParent(manager.gridParent); }
        return f;
    }
}