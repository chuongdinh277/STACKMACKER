using UnityEngine;

public class BridgeStep : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("cầu đã chạm player");
            PlayerStack.Instance.PlaceBrick(this.gameObject);
        }
    }
}
