using UnityEngine;

public class BridgeStep : MonoBehaviour
{
    private bool isPlaced = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!isPlaced && other.CompareTag("Player"))
        {
            Debug.Log("cầu đã chạm player");
            bool success = PlayerStack.Instance.PlaceBrick(this.gameObject);

            if (success)
            {
                isPlaced = true;
            }
        }
    }

    public void ResetStep()
    {
        isPlaced = false;
    }
}
