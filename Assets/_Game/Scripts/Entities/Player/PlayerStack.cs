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
    // hàm xử lí va chạm
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StartPoint"))
        {
            PickUpBrick(other.gameObject);
        }
        if (other.CompareTag("Brick"))
        {
            PickUpBrick(other.gameObject);
        }

        if (other.CompareTag("BridgeStep"))
        {
            Debug.Log("đã va chạm với cầu");
            PlaceBrick(other.gameObject);
        }

        if (other.CompareTag("BrickCorner"))
        {
            Debug.Log("va chạm với brickcorner");
            PlayerMovement moveScript = GetComponent<PlayerMovement>();

            if (moveScript != null && moveScript.IsMoving)
            {
                Vector3 cornerPos = other.transform.position;
                transform.position = new Vector3(cornerPos.x, transform.position.y, cornerPos.z);
                moveScript.Redirect(moveScript.MoveVec, cornerPos);
            }
        }
    }

    public void PickUpBrick(GameObject brickObj)
    {
        Collider col = brickObj.GetComponent<Collider>();

        if (col != null) col.enabled = false;

        brickObj.transform.SetParent(stackParent);
        brickObj.transform.localPosition = Vector3.up * (collectedBricks.Count * (brickHeight + 0.002f));
        brickObj.transform.localEulerAngles = new Vector3(-90f, 0, -180f);

        collectedBricks.Add(brickObj);
        UpdatePlayerHeight();
        LevelManager.RemoveTileData(brickObj.transform.position);
    }

    private void PlaceBrick(GameObject bridgeStepObj)
    {
        if (collectedBricks.Count > 0)
        {
            int lastIndex = collectedBricks.Count - 1;

            GameObject brickToDrop = collectedBricks[lastIndex];
            collectedBricks.RemoveAt(lastIndex);

            brickToDrop.transform.SetParent(bridgeStepObj.transform);

            brickToDrop.transform.localPosition = Vector3.zero;
            brickToDrop.transform.localRotation = quaternion.identity;

            UpdatePlayerHeight();
            LevelManager.UpdateTileData(bridgeStepObj.transform.position, "Bricks");
            bridgeStepObj.tag = "Untagged";
        }
    }

    private void UpdatePlayerHeight()
    {
        float newY = collectedBricks.Count * brickHeight;
        modelPlayer.localPosition = new Vector3(0, newY, 0);
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
}
