using System.Collections.Generic;
using Mono.Cecil;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStack : MonoBehaviour
{

    [SerializeField] private Transform modelPlayer;
    [SerializeField] private Transform stackParent;
    [SerializeField] private GameObject brickPrefab;

    [SerializeField] private float brickHeight = 0.2f;

    private List<GameObject> collectedBricks = new List<GameObject>();
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
    }

    public void PickUpBrick(GameObject brickObj)
    {
        Collider col = brickObj.GetComponent<Collider>();

        if (col != null) col.enabled = false;

        brickObj.transform.SetParent(stackParent);

        brickObj.transform.localPosition = Vector3.up * (collectedBricks.Count * brickHeight);
        //brickObj.transform.rotation = Quaternion.identity;

        collectedBricks.Add(brickObj);

        UpdatePlayerHeight();
    }

    // hàm xử lí va chạm khi đi qua cây cầu rỗng
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
        }
    }

    private void UpdatePlayerHeight()
    {
        modelPlayer.localPosition = Vector3.up * (collectedBricks.Count * brickHeight);

    }
    // hàm dùng để dọn dẹp số gạch đang có
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
