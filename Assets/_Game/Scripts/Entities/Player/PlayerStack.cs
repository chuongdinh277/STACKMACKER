using System.Collections.Generic;

using Unity.Mathematics;
using UnityEngine;
public class PlayerStack : MonoBehaviour
{

    [SerializeField] private Transform modelPlayer;
    [SerializeField] private Transform stackParent;
    [SerializeField] private GameObject brickPrefab;

    [SerializeField] private float brickHeight = 0.4f;

    private List<GameObject> collectedBricks = new List<GameObject>();

    public int CollectedBrickCount => collectedBricks.Count;
    public static PlayerStack Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void PickUpBrick(GameObject brickObj)
    {
        Collider col = brickObj.GetComponent<Collider>();

        if (col != null) col.enabled = false;

        brickObj.transform.SetParent(stackParent);
        brickObj.transform.localPosition = Vector3.up * (collectedBricks.Count * (brickHeight + 0.002f));
        brickObj.transform.localEulerAngles = new Vector3(-90f, 0, -180f);

        collectedBricks.Add(brickObj);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.score = collectedBricks.Count;
        }

        UpdatePlayerHeight();
        LevelManager.RemoveTileData(brickObj.transform.position);
    }

    public void ClearStack()
    {
        foreach (GameObject brick in collectedBricks)
        {
            Destroy(brick);
        }

        collectedBricks.Clear();
        UpdatePlayerHeight();
    }
    public void PlaceBrick(GameObject bridgeStepObj)
    {
        if (collectedBricks.Count > 0)
        {
            int lastIndex = collectedBricks.Count - 1;

            GameObject brickToDrop = collectedBricks[lastIndex];
            collectedBricks.RemoveAt(lastIndex);

            brickToDrop.transform.localScale = new Vector3(1f, 1f, 0.5f);
            brickToDrop.transform.SetParent(bridgeStepObj.transform);

            brickToDrop.transform.localPosition = Vector3.zero;
            brickToDrop.transform.localRotation = quaternion.identity;

            UpdatePlayerHeight();
            LevelManager.UpdateTileData(bridgeStepObj.transform.position, TileType.Brick);
            bridgeStepObj.tag = "Untagged";
        }

        else
        {
            PlayerMovement moveScript = GetComponent<PlayerMovement>();
            if (moveScript != null)
            {
                moveScript.StopAtBridge(bridgeStepObj.transform.position);
            }
        }
    }

    private void UpdatePlayerHeight()
    {
        float newY = collectedBricks.Count * brickHeight;
        modelPlayer.localPosition = new Vector3(0, newY, 0);
    }
}
