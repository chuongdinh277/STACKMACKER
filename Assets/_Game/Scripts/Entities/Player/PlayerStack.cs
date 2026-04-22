using System.Collections.Generic;
using Mono.Cecil;
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
            Debug.Log("Va chạm với gạch!"); 
            PickUpBrick(other.gameObject);
        }

        if (other.CompareTag("BridgeStep"))
        {
            Debug.Log("va chạm với cầu");
            PlaceBrick(other.gameObject);
        }

        // if (other.CompareTag("Win"))
        // {
        //     GetComponent<PlayerMovement>().Win();
        // }
    }

    // hàm xử lí va chạm khi nhặt miếng gỗ trên đường
    public void PickUpBrick(GameObject brickObj)
    {
        if (!brickObj.activeSelf) return;
        brickObj.SetActive(false);

        GameObject newBrick = Instantiate(brickPrefab, stackParent);

        newBrick.transform.localPosition = Vector3.up * (collectedBricks.Count * brickHeight);

        collectedBricks.Add(newBrick);

        modelPlayer.localPosition = Vector3.up * (collectedBricks.Count * brickHeight);
    }

    // hàm xử lí va chạm khi đi qua cây cầu rỗng
    private void PlaceBrick(GameObject bridgeStepObj)
    {
        if (collectedBricks.Count > 0)
        {
            int lastIndex = collectedBricks.Count - 1;
            GameObject brickToDrop = collectedBricks[lastIndex];
            collectedBricks.RemoveAt(lastIndex);
            Destroy(brickToDrop);

            modelPlayer.localPosition = Vector3.up * (collectedBricks.Count * brickHeight);


           Transform brickChild = bridgeStepObj.transform.GetChild(0);
           MeshRenderer childMesh = brickChild.GetComponent<MeshRenderer>();

            if (childMesh == null)
            {
                Debug.Log("<screen> lỗi ko tìm được");
            }
            if (childMesh != null)
            {
                childMesh.enabled = true;
            }

            bridgeStepObj.tag = "Untagged";

            Collider bridgeCol = bridgeStepObj.GetComponent<Collider>();
            if (bridgeCol != null)
            {
                bridgeCol.enabled = false;
            }
        }
    }

    // hàm dùng để dọn dẹp số gạch đang có
    public void ClearStack()
    {
        
    }
}
