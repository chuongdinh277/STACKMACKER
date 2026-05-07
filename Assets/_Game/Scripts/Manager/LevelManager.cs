using UnityEngine;
using System.Collections.Generic;


public class LevelManager : MonoBehaviour
{
    [Header("Settings")]
    public string levelFileName = "level_1";
    [Header("Manual References")]
    public PlayerMovement playerMovement; 
    public PlayerStack playerStack;
    
    public Vector3 startPointPos;
    public List<PrefabMapping> prefabPool;

    public static Dictionary<Vector3, TileData> worldMap = new Dictionary<Vector3, TileData>(); 

    public static List<Vector3> stopPoints = new List<Vector3>();

    public static LevelManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void GenerateLevel()
    {

        if (playerMovement != null) playerMovement.enabled = false;
        string currentFileName = "Level_" + GameManager.Instance.currentLevel;
        TextAsset jsonAsset = Resources.Load<TextAsset>(currentFileName);

        if (jsonAsset == null)
        {
            return;
        }

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        worldMap.Clear();

        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCurrentLevel(GameManager.Instance.currentLevel);
        }

        LevelData data = JsonUtility.FromJson<LevelData>(jsonAsset.text);

        Dictionary<TileType, Transform> groups = new Dictionary<TileType, Transform>();

        foreach (TileData t in data.tiles)
        {
            if (!worldMap.ContainsKey(t.position))
            {
                worldMap.Add(t.position, t);
            }

            var mapping =  prefabPool.Find(p => p.type == t.type);
            if (mapping.prefab == null) continue;

            

            GameObject prefab =  mapping.prefab;

            if (!groups.ContainsKey(t.type))
            {
                GameObject g = new GameObject(t.type.ToString());
                g.transform.SetParent(this.transform);
                groups.Add(t.type, g.transform);
            }

            GameObject newTile = Instantiate(mapping.prefab, t.position, Quaternion.Euler(t.rotation));

            if (t.type == TileType.StartPoint)
            {
                Renderer r = newTile.GetComponentInChildren<Renderer>();
                if (r != null)
                {
                    startPointPos = new Vector3(r.bounds.center.x, r.bounds.max.y, r.bounds.center.z);
                }
                else
                {
                    startPointPos = t.position + Vector3.up * 0.5f;
                }
            }

            newTile.name = mapping.prefab.name;
            newTile.layer = t.layer;



            if (!string.IsNullOrEmpty(t.parentTag)) 
            {
                newTile.tag = t.parentTag;
            }


            newTile.transform.SetParent(groups[t.type]);


            
            Transform[] children = newTile.GetComponentsInChildren<Transform>(true);
            for (int i = 1; i < children.Length; i++)
            {
                if (i - 1 < t.childrenTags.Count && !string.IsNullOrEmpty(t.childrenTags[i - 1]))
                {
                    children[i].tag = t.childrenTags[i - 1];
                }
            }
            newTile.isStatic = true;
        }

        DelayedSetup();
    }


    public static void UpdateTileData (Vector3 pos,  TileType newType)
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

    public void NextLevel()
    {
        ExecuteNextLevel();
    }

    private void ExecuteNextLevel()
    {
        if (GameManager.Instance != null)
        {
            int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);
            int nextLevel = GameManager.Instance.currentLevel + 1;


            if (nextLevel > reachedLevel)
            {
                PlayerPrefs.SetInt("ReachedLevel", nextLevel);
                PlayerPrefs.Save();
                Debug.Log("đã lưu level cao nhất" + nextLevel);
            }
            GameManager.Instance.currentLevel++;
            if (GameManager.Instance.currentLevel > 10)
            {
                GameManager.Instance.currentLevel = 1;
            }
            GenerateLevel();

        }
    }
    private void DelayedSetup()
    {
        if (playerMovement != null && playerStack != null)
        {
            playerMovement.enabled = true;
            playerStack.ClearStack();
            playerMovement.OnInit();
        }
    }
}