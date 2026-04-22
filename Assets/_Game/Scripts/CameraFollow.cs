using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerRoot;
    public Transform modelPlayer;

    public Vector3 offset = new Vector3(0f, 10f, -10f);
    public float smoothSpeed = 10f;

    void LateUpdate()
    {
        if (playerRoot == null || modelPlayer == null) return;

        Vector3 targetPos = new Vector3(playerRoot.position.x, modelPlayer.position.y, playerRoot.position.z) + offset;

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
